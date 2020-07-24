using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogViewer.Models.DirectoryTree
{
    public class FileSystemNode : BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private FileSystemInfo info;
        public FileSystemInfo Info
        {
            get { return info; }
            set { SetProperty(ref info, value); }
        }
    }
}
