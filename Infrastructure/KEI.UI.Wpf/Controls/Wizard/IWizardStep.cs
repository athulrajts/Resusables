
using System;

namespace KEI.UI.Wpf.Controls
{
    public interface IWizardStep
    {
        public bool CanGoNext { get; }
        public bool CanGoPrevious { get; }
        public bool CanCancel { get; }
        public bool CanFinish { get; }

        public bool ShowNext { get; }
        public bool ShowPrevious { get; }
        public bool ShowCancel { get; }
        public bool ShowFinish { get; }

        public string Title { get; }
        public Type ViewType { get; set; }
    }
}
