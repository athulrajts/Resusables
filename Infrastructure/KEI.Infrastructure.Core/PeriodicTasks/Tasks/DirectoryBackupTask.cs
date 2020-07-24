using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.PeriodicTasks
{
    public class DirectoryBackupTask : PeriodicTask
    {
        private DirectoryInfo _dir;
        private readonly ILogger _logger;

        public DirectoryBackupTask(ILogManager logManager)
        {
            _logger = logManager.GetLogger();
        }

        private string directory;
        public string Directory
        {
            get { return directory; }
            set { SetProperty(ref directory, value); }
        }

        private string backupDirectory;
        public string BackupDirectory
        {
            get { return backupDirectory; }
            set { SetProperty(ref backupDirectory, value); }
        }

        private int numberOfFilesToBackup;
        public int NumberOfFilesToBackup
        {
            get { return numberOfFilesToBackup; }
            set { SetProperty(ref numberOfFilesToBackup, value); }
        }

        private string searchPattern = "*.*";
        public string SearchPattern
        {
            get { return searchPattern; }
            set { SetProperty(ref searchPattern, value); }
        }

        protected override void InternalExecute()
        {
            var files = new List<FileInfo>(_dir?.GetFiles(SearchPattern, SearchOption.AllDirectories));

            if (!System.IO.Directory.Exists(BackupDirectory))
                System.IO.Directory.CreateDirectory(BackupDirectory);


            if (files.Count > NumberOfFilesToBackup)
            {
                var now = DateTime.Now;

                string zipFile = $"Backup_[{files.Last().CreationTime.ToString("dd-MMM-yyyy_hh_mm_ss")}] - [{now.ToString("dd-MMM-yyyy_hh_mm_ss")}].zip";

                string tempDirectory = $"Backup_[{files.Last().CreationTime.ToString("dd-MMM-yyyy_hh_mm_ss")}] - [{now.ToString("dd-MMM-yyyy_hh_mm_ss")}]_temp";

                System.IO.Directory.CreateDirectory(tempDirectory);

                files.OrderBy(x => x.CreationTime).Take(NumberOfFilesToBackup).ToList().ForEach((file) =>
                {
                    try
                    {
                        file.MoveTo(Path.Combine(tempDirectory, file.Name));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Exception when deleting file {file.Name}", ex);
                    }
                });

                ZipFile.CreateFromDirectory(tempDirectory, Path.Combine(BackupDirectory, zipFile));

                System.IO.Directory.Delete(tempDirectory, true);

            }
        }

        protected override void InitializeParameters()
        {
            _dir = new DirectoryInfo(Directory);
        }
    }
}
