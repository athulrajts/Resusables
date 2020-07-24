using Localizer.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer.ViewModels
{
    public class LocalizerViewModel : BindableBase
    {
        public LocalizerViewModel()
        {
            LoadSolution(@"C:\Users\AmalRaj\Desktop\Framework Test");
        }

        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        public void LoadSolution(string solutionDirectory)
        {
            var dir = Directory.GetDirectories(solutionDirectory, "Properties", SearchOption.AllDirectories);
            foreach (var item in dir)
            {
                Projects.Add(new Project(item));
            }
        }

        private DelegateCommand<TranslationFile> viewTranslationFile ;
        public DelegateCommand<TranslationFile> ViewTranslationFileCommand =>
            viewTranslationFile ?? (viewTranslationFile = new DelegateCommand<TranslationFile>(ExecuteAddViewTranslationFileCommand));

        void ExecuteAddViewTranslationFileCommand(TranslationFile param)
        {
            CommonServiceLocator.ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<ViewTranslationFileEvent>()
                .Publish(param);
        }
    }

    public class ViewTranslationFileEvent : PubSubEvent<TranslationFile> { }
}
