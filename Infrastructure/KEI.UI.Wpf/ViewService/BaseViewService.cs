using System;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Ioc;
using Prism.Services.Dialogs;
using KEI.Infrastructure;
using KEI.Infrastructure.Logging;
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

        /// <summary>
        /// Implementation for <see cref="IViewService.IsBusy"/>
        /// </summary>
        public bool IsBusy => isBusy;

        /// <summary>
        /// Implementation for <see cref="IViewService.Error(string, bool)"/>
        /// </summary>
        /// <param name="error"></param>
        /// <param name="isModal"></param>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.Inform(string, bool)"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isModal"></param>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.Warn(string, bool)"/>
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="isModal"></param>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.Prompt(string, PromptOptions)"/>
        /// </summary>
        /// <param name="confirmMsg"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.PromptWithDefault(string, PromptOptions, PromptResult, TimeSpan)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <param name="defaultResult"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.SetAvailable"/>
        /// </summary>
        public void SetAvailable()
        {
            if (IsBusy == false)
            {
                return;
            }

            loading?.Close();
            isBusy = false;
        }

        /// <summary>
        /// Implementation for <see cref="IViewService.SetBusy(string[])"/>
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.UpdateBusyText(string[])"/>
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// Implementation for <see cref="IViewService.SwitchUser"/>
        /// </summary>
        public virtual void SwitchUser() => ContainerLocator.Container.Resolve<LoginWindow>().ShowDialog();

        /// <summary>
        /// Implementation for <see cref="IViewService.EditObject(object)"/>
        /// </summary>
        /// <param name="o"></param>
        public void EditObject(object o)
        {
            new ObjectEditorWindow(o).ShowDialog();
        }

        /// <summary>
        /// Implementation for <see cref="IViewService.BrowseFile(string, string)"/>
        /// </summary>
        /// <param name="description"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public string BrowseFile(FilterCollection filters = null, string initialDirectory = null)
        {
            initialDirectory ??= AppDomain.CurrentDomain.BaseDirectory;

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
                DefaultDirectory = initialDirectory,
                InitialDirectory = initialDirectory
            };

            foreach (var filter in filters)
            {
                dlg.Filters.Add(new CommonFileDialogFilter(filter.Description, filter.Extenstion));
            }

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Implementation for <see cref="IViewService.BrowseFolder"/>
        /// </summary>
        /// <returns></returns>
        public string BrowseFolder(string initialDirectory = null)
        {
            initialDirectory ??= AppDomain.CurrentDomain.BaseDirectory;

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
                DefaultDirectory = initialDirectory,
                InitialDirectory = initialDirectory
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Implementation for <see cref="IViewService.SaveFile(Action{string}, string)"/>
        /// </summary>
        /// <param name="saveAction"></param>
        /// <param name="filters"></param>
        public void SaveFile(Action<string> saveAction, string filters = "")
        {
            var sfd = new SaveFileDialog();

            if (string.IsNullOrEmpty(filters) == false)
            {
                sfd.Filter = filters;
            }

            if (sfd.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(sfd.FileName) == false)
                {
                    saveAction?.Invoke(sfd.FileName);
                }
            }
        }

    }
}
