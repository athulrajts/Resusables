using ConfigEditor.Dialogs;
using KEI.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;

namespace ConfigEditor.ViewModels
{
    public class MergeViewModel : BindableBase
    {
        private readonly IConfigEditorViewService _viewService;
        private readonly IDialogFactory _dialogFactory;

        public SnapShotDiff Diff { get; set; }
        public SnapShot Merged { get; set; }
        public IPropertyContainer Left { get; set; }
        public IPropertyContainer Right { get; set; }
        public string Name => $"{Path.GetFileNameWithoutExtension(Left.FilePath)} - {Path.GetFileNameWithoutExtension(Right.FilePath)}";
        public string FullName => Name;

        public MergeViewModel(IConfigEditorViewService viewService, IDialogFactory dialogFactory, string left, string right)
        {
            _viewService = viewService;
            _dialogFactory = dialogFactory;

            Compare(left, right);
        }

        private void Compare(string left, string right)
        {
            Left = PropertyContainerBuilder.FromFile(left);
            Right = PropertyContainerBuilder.FromFile(right);

            if (Left is null)
            {
                _viewService.Error($"Invalid Config : {left}");
                return;
            }
            if (Right == null)
            {
                _viewService.Error($"Invalid Config : {right}");
                return;
            }

            Diff = Left.GetSnapShot() - Right.GetSnapShot();

            Merged = new SnapShot();

            foreach (var key in Diff.Keys)
            {
                Merged.Add(key, Diff[key].DataObjectType, Diff[key].Left ?? Diff[key].Right);
            }
        }

        #region Commands

        private DelegateCommand saveCommand;
        public DelegateCommand SaveCommand =>
            saveCommand ??= saveCommand = new DelegateCommand(ExecuteSaveCommand);

        private void ExecuteSaveCommand()
        {
            var results = _dialogFactory.ShowDialog("Save Merged");

            string filePath = results.GetValue(FileDialogKeys.FileName);
            string shape = results.GetValue(SaveMergedFileDialog.Shape);
            bool result = results.GetValue(FileDialogKeys.DialogResult);
            
            if (result)
            {
                IPropertyContainer merged = shape == "Left" ? Left : Right;
                merged.Restore(Merged);
                merged.Store(filePath); 
            }
        }

        private DelegateCommand<SnapShotItem> swapValueCommand;
        public DelegateCommand<SnapShotItem> SwapValueCommand =>
            swapValueCommand ??= new DelegateCommand<SnapShotItem>(ExecuteSwapValueCommand);

        private void ExecuteSwapValueCommand(SnapShotItem parameter)
        {
            object value = parameter.Value.Equals(Diff[parameter.Name].Left)
                ? Diff[parameter.Name].Right
                : Diff[parameter.Name].Left;

            Merged.Update(parameter.Name, value);

            RaisePropertyChanged(nameof(Diff));
        }

        #endregion;

    }
}
