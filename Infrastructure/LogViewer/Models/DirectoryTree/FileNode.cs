using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogViewer.Models.DirectoryTree
{
    public class FileNode : FileSystemNode
    {
        public FileNode(FileInfo info)
        {
            Info = info;
        }
    }
}
