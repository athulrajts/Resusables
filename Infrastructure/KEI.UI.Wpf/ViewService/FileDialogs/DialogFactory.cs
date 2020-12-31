using System;
using System.Collections.Generic;
using System.Windows;
using KEI.Infrastructure;

namespace KEI.UI.Wpf
{
    public class DialogFactory : IDialogFactory
    {
        private readonly Dictionary<string, Type> _dialogs = new Dictionary<string, Type>();

        public void RegisterDialog<TDialog>(string name)
            where TDialog : IDialog, new()
        {
            _dialogs.Add(name, typeof(TDialog));
        }

        public IDataContainer ShowDialog(string dialogName, IDataContainer parameters)
        {
            if (_dialogs.ContainsKey(dialogName) == false)
            {
                MessageBox.Show($"Dialog : \"{dialogName}\" not found !", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);

                return new DataContainer
                {
                    { FileDialogKeys.DialogResult, false }
                };
            }

            IDialog dialog = (IDialog)Activator.CreateInstance(_dialogs[dialogName]);

            if(parameters is null)
            {
                dialog.ShowDialog();
            }
            else
            {
                dialog.ShowDialog(parameters);
            }

            return dialog.Results;
        }
    }
}
