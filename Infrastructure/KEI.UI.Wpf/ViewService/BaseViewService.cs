using CommonServiceLocator;
using KEI.Infrastructure;
using KEI.Infrastructure.Localizer;
using KEI.UI.Wpf.Controls.ObjectEditors;
using KEI.UI.Wpf.Hotkey;
using KEI.UI.Wpf.ViewService.Dialogs;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Events;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Prism.Unity;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace KEI.UI.Wpf.ViewService
{
    public class BaseViewService : IViewService
    {
        protected IDialogService _dialogService;
        protected static ILogger _logger;
        protected static IEventAggregator _eventAggregator;
        protected bool isBusy;

        private Window loading;

        public BaseViewService(IDialogService dialogService, ILogManager logManager, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _logger = logManager.GetLogger();
            _eventAggregator = eventAggregator;
        }

        public bool IsBusy => isBusy;

        public void Error(string error, bool isModal = false)
        {
            _logger.Error(error);

            var parameters = new DialogParameters
            {
                { "message", error  },
                { "title", DialogType.Error },
                { "buttons", PromptOptions.Ok },
            };

            ShowGenericDialog(parameters, isModal);
        }

        public void Inform(string info, bool isModal = false)
        {
            _logger.Info(info);

            var parameters = new DialogParameters
            {
                { "message", info  },
                { "title", DialogType.Info },
                { "buttons", PromptOptions.Ok },
            };

            ShowGenericDialog(parameters, isModal);
        }

        public void Warn(string warning, bool isModal = false)
        {
            _logger.Warn(warning);

            var parameters = new DialogParameters
            {
                { "message", warning  },
                { "title", DialogType.Warning },
                { "buttons", PromptOptions.Ok },
            };

            ShowGenericDialog(parameters, isModal);
        }


        public PromptResult Prompt(string confirmMsg, PromptOptions buttons)
        {
            _logger.Info(confirmMsg);
            ButtonResult result = ButtonResult.None;

            var parameters = new DialogParameters
            {
                { "message", confirmMsg  },
                { "title", DialogType.Confirm },
                { "buttons", buttons },
            };

            _dialogService.ShowDialog(DialogNames.GenericDialog, parameters, r =>
            {
                _logger.Info($"User Confirmation : {r.Result}");
                result = r.Result;
            });

            return (PromptResult)(int)result;
        }

        public PromptResult PromptWithDefault(string message, PromptOptions buttons, PromptResult defaultResult, TimeSpan timeout)
        {
            _logger.Info(message);
            ButtonResult result = ButtonResult.None;

            var parameters = new DialogParameters
            {
                { "message", message  },
                { "title", DialogType.Confirm },
                { "buttons", buttons },
                { "defaultResult", defaultResult},
                { "timeout", timeout }
            };

            _dialogService.ShowDialog(DialogNames.GenericDialog, parameters, r =>
            {
                _logger.Info($"User Confirmation : {r.Result}");
                result = r.Result;
            });

            return (PromptResult)(int)result;
        }

        public void SetAvailable()
        {
            if (IsBusy == false)
            {
                return;
            }

            loading?.Close();
            isBusy = false;
        }

        public void SetBusy(params string[] msg)
        {
            if (IsBusy)
            {
                return;
            }

            loading = new Window
            {
                Height = Application.Current.MainWindow.ActualHeight,
                Width = Application.Current.MainWindow.ActualWidth,
                AllowsTransparency = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowStyle = WindowStyle.None,
                WindowState = Application.Current.MainWindow.WindowState,
                Owner = Application.Current.MainWindow,
                Background = Brushes.Transparent,
                Content = new LoadingOverlay { DataContext = new LoadingOverlayViewModel { LoadingText = string.Join(Environment.NewLine, msg) } }
            };

            loading.Show();

            isBusy = true;
        }

        public void UpdateBusyText(params string[] msg)
        {
            if (msg == null || IsBusy == false)
            {
                return;
            }

            _eventAggregator.GetEvent<UpdateBusyText>().Publish(string.Join(Environment.NewLine, msg));
        }

        public virtual void SwitchUser() => ServiceLocator.Current.GetInstance<LoginWindow>().ShowDialog();

        public void EditObject(object o)
        {
            new ObjectEditorWindow(o).ShowDialog();
        }

        public string BrowseFile(string description = "", string filters = "")
        {
            var dlg = new CommonOpenFileDialog();
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            dlg.IsFolderPicker = false;
            if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(filters))
            {
                dlg.Filters.Add(new CommonFileDialogFilter(description, filters));
            }
            dlg.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        public string BrowseFolder()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            dlg.IsFolderPicker = true;
            dlg.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        public void SaveFile(Action<string> saveAction, string filters = "")
        {
            var sfd = new SaveFileDialog();

            if (!string.IsNullOrEmpty(filters))
                sfd.Filter = filters;

            if (sfd.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(sfd.FileName))
                    saveAction(sfd.FileName);
            }
        }

        private void ShowGenericDialog(DialogParameters parameters, bool isModal)
        {
            if (isModal)
            {
                _dialogService.ShowDialog(DialogNames.GenericDialog, parameters, r => { });
            }
            else
            {
                _dialogService.Show(DialogNames.GenericDialog, parameters, r => { });
            }
        }

        protected static class DialogNames
        {
            public static readonly string GenericDialog = typeof(GenericDialog).Name;

        }
    }

    internal class UpdateBusyText : PubSubEvent<string> { }

    public static class ContainerRegisteryExtensions
    {
        public static void RegisterUIServices(this IContainerRegistry registry)
        {
            registry.RegisterSingleton<IViewService, BaseViewService>();
            registry.RegisterSingleton<IHotkeyService, HotkeyService>();
            registry.RegisterDialog<GenericDialog>();
            registry.RegisterDialog<ConfigsChangedDialog>();
            registry.RegisterDialog<LoadingOverlay>();
            registry.RegisterInstance<IStringLocalizer>(new ResourceManagerStringLocalizer(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().GetName().Name);

            Infrastructure.ViewService.Service = registry.GetContainer().TryResolve<IViewService>();
        }
    }
}
