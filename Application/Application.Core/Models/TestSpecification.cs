using Prism.Mvvm;

namespace Application.Core.Models
{
    public class TestSpecification : BindableBase
    {
        private string trackingId;
        public string TrackingId
        {
            get { return trackingId; }
            set { SetProperty(ref trackingId, value); }
        }

        private double minTransmittance;
        public double MinTransmittance
        {
            get { return minTransmittance; }
            set { SetProperty(ref minTransmittance, value); }
        }

        private double maxTransmittance;
        public double MaxTransmittance
        {
            get { return maxTransmittance; }
            set { SetProperty(ref maxTransmittance, value); }
        }
    }
}
