using KEI.Infrastructure.Logging;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Xml;
using KEI.Infrastructure;
using System.Windows.Data;
using System.Windows.Documents;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.CompilerServices;
using System.IO;
using LogViewer.Models.DirectoryTree;
using Prism.Commands;
using LogViewer.Parsers;
using System.Linq;

namespace LogViewer.ViewModels
{
    public class LogViewerWindowViewModel : BindableBase
    {
        public ILogParser Parser = new PatternAppenderLogParser();

        public LogViewerWindowViewModel()
        {

            ViewDebug = ViewInfo = ViewWarn = ViewError = ViewFatal = true;

            Directory.Add(new DirectoryNode(new DirectoryInfo(@"C:\Users\AmalRaj\Desktop\Framework Test\Build\Debug\"), "*.slog"));

        }


        private ListCollectionView logs = new ListCollectionView(new List<LogEvent>());
        public ListCollectionView Logs
        {
            get { return logs; }
            set { SetProperty(ref logs, value); }
        }

        private LogEvent selectedLog;
        public LogEvent SelectedLog
        {
            get { return selectedLog; }
            set { SetProperty(ref selectedLog, value); }
        }

        public ObservableCollection<DirectoryNode> Directory { get; set; } = new ObservableCollection<DirectoryNode>();


        #region Text Filters

        private string messageFilter = string.Empty;
        public string MessageFilter
        {
            get { return messageFilter; }
            set { SetTextFilter(ref messageFilter, value); }
        }

        private string methodFilter = string.Empty;
        public string MethodFilter
        {
            get { return methodFilter; }
            set { SetTextFilter(ref methodFilter, value); }
        }

        private string levelFilter = string.Empty;
        public string LevelFilter
        {
            get { return levelFilter; }
            set { SetTextFilter(ref levelFilter, value); }
        }

        private string timeFilter = string.Empty;
        public string TimeFilter
        {
            get { return timeFilter; }
            set { SetTextFilter(ref timeFilter, value); }
        }

        private string lineFilter = string.Empty;
        public string LineFilter
        {
            get { return lineFilter; }
            set { SetTextFilter(ref lineFilter, value); }
        }

        private string fileFilter = string.Empty;
        public string FileFilter
        {
            get { return fileFilter; }
            set { SetTextFilter(ref fileFilter, value); }
        }

        private void SetTextFilter(ref string storage, string value, [CallerMemberName] string property = "")
        {
            if (SetProperty(ref storage, value))
            {
                Logs.Filter = e =>
                {
                    var evt = e as LogEvent;
                    return evt.Message.Contains(MessageFilter, StringComparison.OrdinalIgnoreCase) &&
                    evt.MethodName.Contains(MethodFilter, StringComparison.OrdinalIgnoreCase) &&
                    evt.Level.ToString().Contains(LevelFilter, StringComparison.OrdinalIgnoreCase) &&
                    evt.Time.ToString().Contains(TimeFilter) &&
                    evt.LineNumber.ToString().Contains(LineFilter) &&
                    evt.FileName.Contains(FileFilter, StringComparison.OrdinalIgnoreCase);
                };
            }
        }

        #endregion

        #region Level Filters

        private bool viewDebug;
        public bool ViewDebug
        {
            get { return viewDebug; }
            set { SetLevelFilter(ref viewDebug, value, LogLevel.Debug); }
        }

        private bool viewInfo;
        public bool ViewInfo
        {
            get { return viewInfo; }
            set { SetLevelFilter(ref viewInfo, value, LogLevel.Info); }
        }

        private bool viewWarn;
        public bool ViewWarn
        {
            get { return viewWarn; }
            set { SetLevelFilter(ref viewWarn, value, LogLevel.Warn); }
        }

        private bool viewError;
        public bool ViewError
        {
            get { return viewError; }
            set { SetLevelFilter(ref viewError, value, LogLevel.Error); }
        }

        private bool viewFatal;
        public bool ViewFatal
        {
            get { return viewFatal; }
            set { SetLevelFilter(ref viewFatal, value, LogLevel.Fatal); }
        }

        public LogLevel Filter { get; set; }

        private void SetLevelFilter(ref bool storage, bool value, LogLevel level, [CallerMemberName] string property = "")
        {
            if (SetProperty(ref storage, value, property))
            {
                Filter = value ? Filter | level : Filter ^ level;
                Logs.Filter = e => Filter.HasFlag((e as LogEvent).Level);
            }
        }

        #endregion

        private List<LogEvent> ReadFile(string path)
        {
            var list = new List<LogEvent>();

            if (File.Exists(path) == false)
                return list;

            //var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            //using var reader = XmlReader.Create(path, settings);

            //while (reader.ReadToFollowing(nameof(LogEvent)))
            //{
            //    list.Add(reader.ReadObjectXML<LogEvent>());
            //}

            //return list;

            return Parser.Parse(path).ToList();
        }

        private DelegateCommand<string> openFileCommand;
        public DelegateCommand<string> OpenFileCommand 
            => openFileCommand ??= new DelegateCommand<string>(ExecuteOpenFileCommand);

        void ExecuteOpenFileCommand(string parameter)
        {
            var prevFilter = Logs.Filter;
            Logs = new ListCollectionView(ReadFile(parameter))
            {
                Filter = prevFilter
            };
        }
    }
}
