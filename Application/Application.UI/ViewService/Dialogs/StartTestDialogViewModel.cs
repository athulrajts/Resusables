using Application.Core.Models;
using KEI.Infrastructure.Validation.Attributes;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;

namespace KEI.UI.Wpf.ViewService
{
    public class StartTestDialogViewModel : BaseValidatingViewModel, IDialogAware
    {

        private DelegateCommand submitDialogCommand;
        public DelegateCommand SubmitDialogCommand
        {
            get
            {
                if (submitDialogCommand == null)
                {
                    submitDialogCommand = new DelegateCommand(() => RaiseRequestClose(new DialogResult(ButtonResult.OK))).ObservesCanExecute(() => HasError);
                }
                return submitDialogCommand;
            }
        }

        private DelegateCommand closeDialogCommand;
        public DelegateCommand CloseDialogCommand
        {
            get
            {
                if (closeDialogCommand == null)
                {
                    closeDialogCommand = new DelegateCommand(() => RaiseRequestClose(new DialogResult(ButtonResult.Cancel)));
                }
                return closeDialogCommand;
            }
        }

        #region IDialogAware Members
        public string Title => "Start Test";
        public event Action<IDialogResult> RequestClose;
        public virtual void RaiseRequestClose(IDialogResult dialogResult) => RequestClose?.Invoke(dialogResult);
        public virtual void OnDialogClosed() { }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Specification = parameters.GetValue<TestSpecification>("Spec");
            maxTransmittance = specification.MaxTransmittance;
            minTransmittance = specification.MinTransmittance;
        }

        public bool CanCloseDialog() => true;

        #endregion

        #region Properties

        private TestSpecification specification;
        public TestSpecification Specification
        {
            get { return specification; }
            set { SetProperty(ref specification, value); }
        }

        private string trackingId;
        private double maxTransmittance;
        private double minTransmittance;

        [NotEmpty]
        public string TrackingID
        {
            get => trackingId;
            set => SetProperty(ref trackingId, value, () => specification.TrackingId = value);
        }

        [InclusiveRange(85, 110)]
        public double MaxTransmittance
        {
            get { return maxTransmittance; }
            set
            {
                SetProperty(ref maxTransmittance, value, () => 
                {
                    specification.MaxTransmittance = value;
                    RaisePropertyChanged(nameof(MinTransmittance));
                });
            }
        }

        [InclusiveRange(50, 84)]
        public double MinTransmittance
        {
            get { return minTransmittance; }
            set
            {
                SetProperty(ref minTransmittance, value, () => 
                {
                    specification.MinTransmittance = value ;
                    RaisePropertyChanged(nameof(MaxTransmittance));
                });
            }
        }

        #endregion
    }
}
