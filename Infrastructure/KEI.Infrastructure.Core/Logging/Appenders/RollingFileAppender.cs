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

        /// <summary>
        /// Base path.
        /// Latest log file will always but this one.
        /// </summary>
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

        /// <summary>
        /// Decides when a new log file is created
        /// </summary>
        public RollingMode Mode { get; set; } = RollingMode.Time;

        /// <summary>
        /// For every log that comes, make sure we can write to current file
        /// If not rename the existing file and create a new one.
        /// </summary>
        /// <param name="msg"></param>
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
                /// Log.slog => Log [22-Jul-2020] + 10MB.slog
                ///              ^       ^           ^
                ///              |       |           |
                ///              1       2           3
                /// 1. Base Name
                /// 2. Date of creation of file
                /// 3. Rolling size in megabytes
                /// File will contain logs of <see cref="RollingSize"/> Megabytes
                /// starting from <see cref="FileSystemInfo.CreationTime"/>
                newFileName = $"{fileNameWithoutExt} [{fileInfo.CreationTime::yyyy-MMM-dd-HH-mm}] + {RollingSize}MB{ext}";
            }
            else if (Mode == RollingMode.Time)
            {
                /// Log.slog => Log [22-Jul-2020].slog if <see cref="RollingInterval"/> == 1
                /// Log.slog => Log [22-Jul-2020] + 2D.slog
                ///              ^       ^          ^
                ///              |       |          |
                ///              1       2          3
                /// 1. Base Name
                /// 2. Date of Creation of file
                /// 3. Rolling interval in days
                /// File will contain logs for <see cref="RollingInterval"/> number of days
                /// starting from <see cref="FileSystemInfo.CreationTime"/>
                newFileName = $"{fileNameWithoutExt} [{fileInfo.CreationTime:dd-MMM-yyyy}]{(RollingInterval > 1 ? $" + {RollingInterval}D" : "")}{ext}";
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

        /// <summary>
        /// Write some data to the begining of the file.
        /// One use case might be to write some necessary information to
        /// parse the log file by a log viewer.
        /// </summary>
        /// <param name="stream"></param>
        private void WriteMetaData(FileStream stream)
        {
            using var writer = new StreamWriter(stream);
            WriteMetaDataInternal(writer);
        }

        /// <summary>
        /// Classes inhertting us can override the method to write specialized metadata.
        /// By default it writes nothing.
        /// Implementers should not close the StreamWritter as it's already handles by use
        /// </summary>
        /// <param name="writter"></param>
        protected virtual void WriteMetaDataInternal(StreamWriter writter) { }
    }

    public enum RollingMode
    {
        Size,
        Time,
    }
}
