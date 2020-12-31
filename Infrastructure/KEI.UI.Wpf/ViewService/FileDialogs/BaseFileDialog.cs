using Microsoft.WindowsAPICodePack.Dialogs;
using KEI.Infrastructure;

namespace KEI.UI.Wpf.ViewService
{
    public abstract class BaseFileDialog<T> : IDialog
        where T : CommonFileDialog, new()
    {
        public IDataContainer Parameters = new DataContainer();

        public T Dialog { get; }

        public IDataContainer Results { get; }

        public BaseFileDialog()
        {
            Dialog = new();

            Results = new DataContainer();

            SetDefaultParameters();

            PopulateControls();
        }

        protected abstract void InitializeDialog();

        protected virtual void PopulateControls() { }

        protected abstract void PopulateResults();

        protected virtual void SetDefaultParameters() { }

        public void ShowDialog()
        {
            InitializeDialog();

            var result = Dialog.ShowDialog();

            Results.PutValue(FileDialogKeys.DialogResult, result == CommonFileDialogResult.Ok);
            
            if (result == CommonFileDialogResult.Ok)
            {
                PopulateResults(); 
            }
        }

        public void ShowDialog(IDataContainer parameters)
        {
            Parameters = parameters;

            ShowDialog();
        }
    }
}
