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
        private readonly IViewService _viewService;
        public ILogParser Parser = new PatternAppenderLogParser();

        public LogViewerWindowViewModel(IViewService viewService)
        {
            _viewService = viewService;

            LogCount.Add(LogLevel.Debug, 0);
            LogCount.Add(LogLevel.Info, 0);
            LogCount.Add(LogLevel.Warn, 0);
            LogCount.Add(LogLevel.Error, 0);
            LogCount.Add(LogLevel.Fatal, 0);

            Directory = new ObservableCollection<DirectoryNode>();

            ViewDebug = ViewInfo = ViewWarn = ViewError = ViewFatal = true;

            Directory.Add(new DirectoryNode(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory), "*.slog") { IsExpanded = true });

        }

        #region Properties

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

        public ObservableCollection<DirectoryNode> Directory { get; set; }

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
                Filter ^= level;
                Logs.Filter = e => Filter.HasFlag((e as LogEvent).Level);
            }
        }

        #endregion

        #region Column Filters

        private bool viewLineNumber = true;
        public bool ViewLineNumber
        {
            get { return viewLineNumber; }
            set { SetProperty(ref viewLineNumber, value); }
        }

        private bool viewLogLevel = true;
        public bool ViewLogLevel
        {
            get { return viewLogLevel; }
            set { SetProperty(ref viewLogLevel, value); }
        }

        private bool viewDateTime = true;
        public bool ViewDateTime
        {
            get { return viewDateTime; }
            set { SetProperty(ref viewDateTime, value); }
        }

        private bool viewFileName = true;
        public bool ViewFileName
        {
            get { return viewFileName; }
            set { SetProperty(ref viewFileName, value); }
        }

        private bool viewMessage = true;
        public bool ViewMessage
        {
            get { return viewMessage; }
            set { SetProperty(ref viewMessage, value); }
        }

        private bool viewMethod = true;
        public bool ViewMethod
        {
            get { return viewMethod; }
            set { SetProperty(ref viewMethod, value); }
        }

        #endregion

        #region Log Count

        public Dictionary<LogLevel, int> LogCount { get; set; } = new Dictionary<LogLevel, int>();

        private int totalLogCount;
        public int TotalLogCount
        {
            get { return totalLogCount; }
            set { SetProperty(ref totalLogCount, value); }
        }

        #endregion

        #endregion

        #region Open File Command

        private DelegateCommand<string> openFileCommand;
        public DelegateCommand<string> OpenFileCommand
            => openFileCommand ??= new DelegateCommand<string>(ExecuteOpenFileCommand);

        void ExecuteOpenFileCommand(string parameter)
        {
            var prevFilter = Logs.Filter;

            var logItems = ReadFile(parameter);

            TotalLogCount = 0;

            foreach (var item in (int [])Enum.GetValues(typeof(LogLevel)))
            {
                LogCount[(LogLevel)item] = logItems.Where(x => (int)x.Level == item).Count();
                TotalLogCount += LogCount[(LogLevel)item];
            }

            RaisePropertyChanged(nameof(LogCount));

            Logs = new ListCollectionView(logItems)
            {
                Filter = prevFilter
            };
        }

        private List<LogEvent> ReadFile(string path)
        {
            var list = new List<LogEvent>();

            if (File.Exists(path) == false)
                return list;

            return Parser.Parse(path).ToList();
        }

        #endregion

        #region Browse Folder Command

        private DelegateCommand browseFolderCommand;
        public DelegateCommand BrowseFolderCommand
            => browseFolderCommand ??= new DelegateCommand(ExecuteBrowseFolderCommand);

        void ExecuteBrowseFolderCommand()
        {
            var folderPath = _viewService.BrowseFolder();

            if (string.IsNullOrEmpty(folderPath) ||
               System.IO.Directory.Exists(folderPath) == false)
            {
                return;
            }

            Directory.Clear();
            Directory.Add(new DirectoryNode(new DirectoryInfo(folderPath), "*.slog") { IsExpanded = true });
        }

        #endregion

        #region Browse File Command

        private DelegateCommand browseFileCommand;
        public DelegateCommand BrowseFileCommand 
            => browseFileCommand ??= new DelegateCommand(ExecuteBrowseFileCommand);

        void ExecuteBrowseFileCommand()
        {
            var filePath = _viewService.BrowseFile("Log files", "slog");

            if (string.IsNullOrEmpty(filePath) || 
                File.Exists(filePath) == false)
            {
                return;
            }

            ExecuteOpenFileCommand(filePath);
        }

        #endregion
    }
}
