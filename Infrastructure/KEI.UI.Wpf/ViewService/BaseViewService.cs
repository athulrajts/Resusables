using System;
using System.Windows;
using System.Reflection;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Ioc;
using Prism.Services.Dialogs;
using KEI.Infrastructure;
using KEI.Infrastructure.Logging;
using KEI.Infrastructure.Localizer;
using KEI.UI.Wpf.Hotkey;
using KEI.UI.Wpf.ViewService.Dialogs;
using KEI.UI.Wpf.Controls.ObjectEditors;
using KEI.UI.Wpf.ViewService.Views;
using KEI.UI.Wpf.ViewService.ViewModels;

namespace KEI.UI.Wpf.ViewService
{
    public class BaseViewService : IViewService
    {
        protected bool isBusy;

        private Window loading;

        public bool IsBusy => isBusy;

        public void Error(string error, bool isModal = true)
        {
            Logger.Error(error);

            var parameters = new DialogParameters
            {
                { "message", error  },
                { "title", DialogType.Error },
                { "buttons", PromptOptions.Ok },
            };

            ThreadingHelper.DispatcherInvoke(() =>
            {
                var host = new DialogWindowHost<GenericDialog>(parameters);

                host.ShowDialog(isModal);
            });
        }

        public void Inform(string info, bool isModal = true)
        {
            Logger.Info(info);

            var parameters = new DialogParameters
            {
                { "message", info  },
                { "title", DialogType.Info },
                { "buttons", PromptOptions.Ok },
            };

            ThreadingHelper.DispatcherInvoke(() =>
            {
                var host = new DialogWindowHost<GenericDialog>(parameters);

                host.ShowDialog(isModal);
            });
        }

        public void Warn(string warning, bool isModal = true)
        {
            Logger.Warn(warning);

            var parameters = new DialogParameters
            {
                { "message", warning  },
                { "title", DialogType.Warning },
                { "buttons", PromptOptions.Ok },
            };

            ThreadingHelper.DispatcherInvoke(() =>
            {
                var host = new DialogWindowHost<GenericDialog>(parameters);

                host.ShowDialog(isModal);
            });

        }


        public PromptResult Prompt(string confirmMsg, PromptOptions buttons)
        {
            Logger.Info(confirmMsg);

            var parameters = new DialogParameters
            {
                { "message", confirmMsg  },
                { "title", DialogType.Confirm },
                { "buttons", buttons },
            };

            var host = new DialogWindowHost<GenericDialog>(parameters);

            host.ShowDialog();

            return host.Result;
        }

        public PromptResult PromptWithDefault(string message, PromptOptions buttons, PromptResult defaultResult, TimeSpan timeout)
        {
            Logger.Info(message);

            var parameters = new DialogParameters
            {
                { "message", message  },
                { "title", DialogType.Confirm },
                { "buttons", buttons },
                { "defaultResult", defaultResult},
                { "timeout", timeout }
            };

            var host = new DialogWindowHost<GenericDialog>(parameters);

            host.ShowDialog();

            return host.Result;
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

            IDialogParameters parameters = new DialogParameters
            {
                { "text", string.Join(Environment.NewLine, msg) }
            };

            loading = new OverlayWindow<LoadingOverlay, LoadingOverlayViewModel>(parameters);

            loading.Show();

            isBusy = true;
        }

        public void UpdateBusyText(params string[] msg)
        {
            if (msg == null || IsBusy == false)
            {
                return;
            }

            if (loading?.DataContext is LoadingOverlayViewModel vm)
            {
                vm.SetBusyText(string.Join(Environment.NewLine, msg));
            }
        }

        public virtual void SwitchUser() => ContainerLocator.Container.Resolve<LoginWindow>().ShowDialog();

        public void EditObject(object o)
        {
            new ObjectEditorWindow(o).ShowDialog();
        }

        public string BrowseFile(string description = "", string filters = "")
        {
            var dlg = new CommonOpenFileDialog
            {
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true,
                IsFolderPicker = false,
                DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

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
            var dlg = new CommonOpenFileDialog
            {
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true,
                IsFolderPicker = true,
                DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

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

    }

    public static class ContainerRegisteryExtensions
    {
        public static void RegisterUIServices(this IContainerRegistry registry)
        {
            registry.RegisterSingleton<IViewService, BaseViewService>();
            registry.RegisterSingleton<IHotkeyService, HotkeyService>();
            registry.RegisterDialog<ConfigsChangedDialog>();
            registry.RegisterInstance<IStringLocalizer>(new ResourceManagerStringLocalizer(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().GetName().Name);

            Infrastructure.ViewService.Service = ContainerLocator.Container.Resolve<IViewService>();
        }
    }
}
