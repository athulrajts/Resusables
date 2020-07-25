using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace LogViewer.Models.DirectoryTree
{
    public class DirectoryNode : FileSystemNode
    {
        private DirectoryInfo Dir => Info as DirectoryInfo;

        private static readonly FileNode dummyFile = new FileNode(new FileInfo("Loading.."));
        private readonly string _pattern;

        public DirectoryNode(DirectoryInfo info, string pattern = "*.*")
        {
            Info = info;
            _pattern = pattern;

            HasChildren = info.EnumerateFileSystemInfos().Where(x => pattern.Contains(x.Extension)).Count() > 0;

            if (HasChildren)
            {
                ChildNodes.Add(dummyFile);
            }

        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value, OnExpandedChanged); }
        }

        private bool hasChildren;
        public bool HasChildren
        {
            get { return hasChildren; }
            set { SetProperty(ref hasChildren, value); }
        }

        private void OnExpandedChanged()
        {
            if(IsExpanded)
            {
                LoadChildren();
            }
            else
            {
                UnloadChildren();
            }
        }

        private void LoadChildren()
        {
            ChildNodes.Remove(dummyFile);
            foreach (var item in Dir.EnumerateDirectories())
            {
                ChildNodes.Add(new DirectoryNode(item, _pattern));
            }
            foreach (var item in Dir.EnumerateFiles(_pattern))
            {
                ChildNodes.Add(new FileNode(item));
            }
        }

        private void UnloadChildren()
        {
            ChildNodes.Clear();
            ChildNodes.Add(dummyFile);
        }

        public ObservableCollection<FileSystemNode> ChildNodes { get; set; } = new ObservableCollection<FileSystemNode>();
    }
}
