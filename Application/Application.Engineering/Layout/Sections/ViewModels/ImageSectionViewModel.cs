using Application.Core;
using Application.Core.Interfaces;
using KEI.Infrastructure.Prism;
using Prism.Events;
using Prism.Mvvm;

namespace Application.Engineering.Layout.Sections.ViewModels
{
    [RegisterSingleton]
    public class ImageSectionViewModel : BindableBase
    {
        private string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set { SetProperty(ref imagePath, value); }
        }           
        public ImageSectionViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TestExecuted>().Subscribe(result => ImagePath = result.Item2, ThreadOption.BackgroundThread, false, (r) => r.Item1 == ApplicationMode.Engineering);
        }
    }
}
