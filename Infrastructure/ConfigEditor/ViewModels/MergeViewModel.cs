﻿using ConfigEditor.Models;
using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Threading.Tasks;

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

        public string Name => $"{Path.GetFileNameWithoutExtension(LeftPath)} - {Path.GetFileNameWithoutExtension(RightPath)}";

        public string FullName => Name;


        private readonly IViewService _viewService;
        public MergeViewModel(IViewService viewService, string left, string right)
        {
            _viewService = viewService;
            LeftPath = left;
            RightPath = right;

            if(CanExecuteRefreshMergeCommand())
            {
                _viewService.SetBusy();

                Task.Run(() =>
                {
                    ExecuteRefreshMergeCommand();
                    _viewService.SetAvailable();
                });

            }

            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(FullName));

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
            var left = PropertyContainerBuilder.FromFile(LeftPath) ?? DataContainer.FromFile(LeftPath).ToPropertyContainer();
            var right = PropertyContainerBuilder.FromFile(RightPath) ?? DataContainer.FromFile(RightPath).ToPropertyContainer();

            if(left == null || right == null)
            {
                _viewService.Error("Invalid Config");
                return;
            }

            Merge = new DataContainerMerger(left, right);
        }
    }
}
