using Application.Core;
using Application.Core.Interfaces;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Engineering.Layout.Sections.ViewModels
{
    [RegisterSingleton]
    public class ResultSectionViewModel : BindableBase
    {
        private List<TestResult> results;
        public List<TestResult> Results
        {
            get { return results; }
            set { SetProperty(ref results, value); }
        }

        private bool isPass;
        public bool IsPass
        {
            get { return isPass; }
            set { SetProperty(ref isPass, value); }
        }

        private string resultText;
        public string ResultText
        {
            get { return resultText; }
            set { SetProperty(ref resultText, value); }
        }

        public ResultSectionViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TestExecuted>().Subscribe(result =>
            {
                Results = result.Item3;
                IsPass = !Results.Any(x => x.IsPass == false);
                ResultText = IsPass ? "PASS" : "FAIL";
            }, ThreadOption.BackgroundThread, false,
            (result) => result.Item1 == ApplicationMode.Engineering);
        }
    }
}
