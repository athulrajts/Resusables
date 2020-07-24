using ConfigEditor.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigEditor.ViewModels
{
    public class MergeViewModel : BindableBase
    {
        private DataContainerMerger merge;
        public DataContainerMerger Merge
        {
            get { return merge; }
            set { SetProperty(ref merge, value); }
        }

        private bool showDifferentItems;
        public bool ShowDifferentItems
        {
            get { return showDifferentItems; }
            set { SetProperty(ref showDifferentItems, value, () => Merge.ShowDifferentItems(showDifferentItems)); }
        }

        private MergeOption mergedShape;
        public MergeOption MergedShape
        {
            get { return mergedShape; }
            set { SetProperty(ref mergedShape, value); }
        }

        private string leftPath;
        public string LeftPath
        {
            get { return leftPath; }
            set { SetProperty(ref leftPath, value); }
        }

        private string rightPath;
        public string RightPath
        {
            get { return rightPath; }
            set { SetProperty(ref rightPath, value); }
        }

        private readonly IViewService _viewService;
        public MergeViewModel(IViewService viewService)
        {
            _viewService = viewService;
            MergedShape = MergeOption.Right;
        }

        private DelegateCommand swapLeftRightCommand;
        public DelegateCommand SwapLeftRightCommand =>
            swapLeftRightCommand ?? (swapLeftRightCommand = new DelegateCommand(ExecuteSwapLeftRightCommand));

        void ExecuteSwapLeftRightCommand() => Merge.Swap();

        private DelegateCommand mergeCommand;
        public DelegateCommand MergeCommand =>
            mergeCommand ?? (mergeCommand = new DelegateCommand(ExecuteMergeCommand));

        void ExecuteMergeCommand()
        {
            var mergedDC = Merge.Merge(MergedShape);

            _viewService.SaveFile(filePath => mergedDC.Store(filePath));
        }

        private DelegateCommand refreshMergeCommand;
        public DelegateCommand RefreshMergeCommand =>
            refreshMergeCommand ?? (refreshMergeCommand = new DelegateCommand(ExecuteRefreshMergeCommand, CanExecuteRefreshMergeCommand)
            .ObservesProperty(() => LeftPath)
            .ObservesProperty(() => RightPath));

        private bool CanExecuteRefreshMergeCommand() 
            => !(string.IsNullOrEmpty(LeftPath) || string.IsNullOrEmpty(RightPath));

        void ExecuteRefreshMergeCommand()
        {
            var left = PropertyContainerBuilder.FromFile(LeftPath);
            var right = PropertyContainerBuilder.FromFile(RightPath);

            if(left == null || right == null)
            {
                _viewService.Error("Invalid Config");
                return;
            }

            Merge = new DataContainerMerger(PropertyContainerBuilder.FromFile(LeftPath), PropertyContainerBuilder.FromFile(RightPath));
        }
    }
}
