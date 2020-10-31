using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using KEI.Infrastructure.Logging;

namespace KEI.Infrastructure.PeriodicTasks
{
    public class DirectoryCleanupTask : PeriodicTask
    {

        private DirectoryInfo _dir;

        private string directory;
        public string Directory
        {
            get { return directory; }
            set { SetProperty(ref directory, value); }
        }

        private int numberOfFilesToDelete;
        public int NumberOfFilesToDelete
        {
            get { return numberOfFilesToDelete; }
            set { SetProperty(ref numberOfFilesToDelete, value); }
        }

        private string searchPattern = "*.*";
        public string SearchPattern
        {
            get { return searchPattern; }
            set { SetProperty(ref searchPattern, value); }
        }

        private int maxFiles;
        public int MaxFiles
        {
            get { return maxFiles; }
            set { SetProperty(ref maxFiles, value); }
        }

        protected override void InternalExecute()
        {
            var files = new List<FileInfo>(_dir?.GetFiles(SearchPattern, SearchOption.AllDirectories));

            if (files.Count > MaxFiles)
            {
                files.OrderBy(x => x.CreationTime).Take(NumberOfFilesToDelete).ToList().ForEach((file) => 
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Exception when deleting file {file.Name}", ex);
                    }
                });
            }

        }

        protected override void InitializeParameters()
        {
            _dir = new DirectoryInfo(Directory);
        }
    }
}
