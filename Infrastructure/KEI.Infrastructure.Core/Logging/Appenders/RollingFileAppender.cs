using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KEI.Infrastructure.Logging
{
    public abstract class RollingFileAppender : BaseAppender
    {
        private readonly object writeLock = new object();
        private const long BYTE_TO_MB_FACTOR = 1000000;
        private Regex fileNameRegex;
        private string fileNameWithoutExt;
        private string ext;
        private string directory;
        
        protected FileInfo fileInfo;

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                if (filePath == value)
                {
                    return;
                }

                fileInfo = new FileInfo(value);
                if (fileInfo.Exists == false)
                {
                    WriteMetaData(fileInfo.Create());
                    fileInfo.Refresh();
                }

                filePath = value;
                fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                fileNameRegex = new Regex($@"{fileNameWithoutExt}(\d+)");
                directory = fileInfo.DirectoryName;
                ext = fileInfo.Extension;
            }
        }

        /// <summary>
        /// Size in Megabytes
        /// Minimum size is fixed as 10
        /// </summary>
        private int rollingSize = 10;
        public int RollingSize
        {
            get => rollingSize;
            set
            {
                if (value == rollingSize || value < 10)
                {
                    return;
                }

                rollingSize = value;
            }
        }

        /// <summary>
        /// Time in days
        /// Minimum interval is 1 Day
        /// </summary>
        private int rollingInterval = 1;
        public int RollingInterval
        {
            get => rollingInterval;
            set
            {
                if(value == rollingInterval || value < 1)
                {
                    return;
                }

                rollingInterval = value;
            }
        }

        public RollingMode Mode { get; set; } = RollingMode.Time;

        protected override void ProcessLogInternal(LogEvent msg)
        {
            if (CanRoll())
            {
                Roll();
            }

            lock (writeLock)
            {
                WriteToFile(msg); 
            }
        }

        protected abstract void WriteToFile(LogEvent msg);


        /// <summary>
        /// Determines whether we need to create a new log file
        /// </summary>
        /// <returns></returns>
        private bool CanRoll()
        {
            return Mode switch
            {
                RollingMode.Time => DateTime.Now.DayOfYear > fileInfo.CreationTime.DayOfYear,
                RollingMode.Size => HasOverflowed(),
                _ => false
            };
        }


        /// <summary>
        /// Current log file has reached the limit.
        /// Rename it and start a new one.
        /// </summary>
        private void Roll()
        {
            string newFileName = string.Empty;

            if (Mode == RollingMode.Size)
            {

                int number = 0;

                foreach (var file in Directory.EnumerateFiles(directory, $"{fileNameWithoutExt}*{ext}"))
                {
                    if (fileNameRegex.Match(file) is Match m)
                    {
                        if (int.TryParse(m.Groups[1].Value, out int val))
                        {
                            if (val > number)
                            {
                                number = val;
                            }
                        }
                    }
                }

                /// Log.slog => Log1.slog
                ///              ^ ^
                ///              | |
                ///              1 2
                /// 1. Base Name
                /// 2. Count
                /// Smaller values of count indicate older file
                newFileName = $"{fileNameWithoutExt}{number + 1}{ext}";
            }
            else if (Mode == RollingMode.Time)
            {
                /// Log.slog => Log [22-Jul-2020].slog if <see cref="RollingInterval"/> == 1
                /// Log.slog => Log [22-Jul-2020] + 2.slog
                ///              ^       ^          ^
                ///              |       |          |
                ///              1       2          3
                /// 1. Base Name
                /// 2. Day of Creation of file
                /// 3. Rolling interval in days
                /// File will contain logs for <see cref="RollingInterval"/> number of days
                /// starting from <see cref="FileSystemInfo.CreationTime"/>
                newFileName = $"{fileNameWithoutExt} [{fileInfo.CreationTime:dd-MMM-yyyy}]{(RollingInterval > 1 ? $" + {RollingInterval}" : "")}{ext}";
            }

            /// Rename current file
            fileInfo.MoveTo(Path.Combine(fileInfo.DirectoryName, newFileName));

            /// Create our base file again
            fileInfo = new FileInfo(FilePath);
            WriteMetaData(fileInfo.Create());
            fileInfo.Refresh();
        }

        /// <summary>
        /// Determines whether is file sies > <see cref="RollingSize"/>
        /// </summary>
        /// <returns></returns>
        private bool HasOverflowed()
        {
            fileInfo.Refresh();
            return fileInfo.Length / BYTE_TO_MB_FACTOR > RollingSize;
        }

        private void WriteMetaData(FileStream stream)
        {
            using var writer = new StreamWriter(stream);
            WriteMetaDataInternal(writer);
        }

        protected virtual void WriteMetaDataInternal(StreamWriter writter) { }
    }

    public enum RollingMode
    {
        Size,
        Time,
    }
}
