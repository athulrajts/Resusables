using System;
using KEI.Infrastructure;
using KEI.UI.Wpf.ViewService;
using Prism.Events;
using Prism.Services.Dialogs;

namespace ConfigEditor.Dialogs
{
    public interface IConfigEditorViewService : IViewService
    {
        (string left, string right) BrowseCompairFiles();
    }

    public class ConfigEditorViewService : BaseViewService, IConfigEditorViewService
    {
        public (string left, string right) BrowseCompairFiles()
        {
            var window = new SelectFilesToCompareWindow() { WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen};
            if (window.ShowDialog() == true)
            {
                return (window.LeftFile, window.RightFile);
            }

            return (string.Empty, string.Empty);
        }
    }
}
