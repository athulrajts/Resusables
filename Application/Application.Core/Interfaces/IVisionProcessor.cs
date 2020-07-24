using KEI.Infrastructure.Database;
using KEI.Infrastructure.Service;
using System.Collections.Generic;

namespace Application.Core.Interfaces
{
    [Service("Vision", typeof(Modules.VisionProcessor))]
    public interface IVisionProcessor
    {
        List<TestResult> Test(string imagePath);
        List<TestResult> ReferenceTest(string imagePath);
    }

    public class TestResult : IDatabaseContext
    {
        public string ID { get; set; }
        public bool IsPass { get; set; }
        public double AverageIntensity { get; set; }
        public double AverageIntensityReference { get; set; }
        public double Transmittance { get; set; }
    }
}
