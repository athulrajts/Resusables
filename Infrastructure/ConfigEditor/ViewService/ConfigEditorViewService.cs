﻿using KEI.Infrastructure;
using KEI.UI.Wpf.ViewService;

namespace ConfigEditor.Dialogs
{
    public class ConfigEditorViewService : BaseViewService, IConfigEditorViewService
    {
        public (string left, string right) BrowseCompairFiles()
        {
            var window = new SelectFilesToCompareWindow() { WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen };
            if (window.ShowDialog() == true)
            {
                return (window.LeftFile, window.RightFile);
            }

            return (string.Empty, string.Empty);
        }

        public IDataContainer ShowSaveMergeFileDialog()
        {
            var dlg = new SaveMergedFileDialog();

            dlg.ShowDialog();

            return dlg.Results;
        }

    }
}
