using Prism.Mvvm;
using System;

namespace KEI.UI.Wpf.Controls
{
    public class WizardStep : BindableBase, IWizardStep
    {
        private bool canGoNext;
        public bool CanGoNext
        {
            get { return canGoNext; }
            set { SetProperty(ref canGoNext, value); }
        }

        private bool canGoPrevious;
        public bool CanGoPrevious
        {
            get { return canGoPrevious; }
            set { SetProperty(ref canGoPrevious, value); }
        }

        private bool canCancel;
        public bool CanCancel
        {
            get { return canCancel; }
            set { SetProperty(ref canCancel, value); }
        }

        private bool canFinish;
        public bool CanFinish
        {
            get { return canFinish; }
            set { SetProperty(ref canFinish, value); }
        }

        public bool ShowNext { get; set; }
        public bool ShowPrevious { get; set; }
        public bool ShowCancel { get; set; }
        public bool ShowFinish { get; set; }

        public string Title { get; set; }
        public Type ViewType { get; set; }

    }
}
