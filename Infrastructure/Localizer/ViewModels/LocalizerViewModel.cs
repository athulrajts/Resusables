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
        private readonly ILocalizerViewSerivce _viewService;

        public LocalizerViewModel(ILocalizerViewSerivce viewSerivce)
        {
            _viewService = viewSerivce;

            LoadSolution(@"C:\Users\AmalRaj\Desktop\Framework Test");
        }

        public ObservableCollection<Project> Projects { get; set; } = new ObservableCollection<Project>();

        public void LoadSolution(string solutionDirectory)
        {
            Projects.Clear();

            var dir = Directory.GetDirectories(solutionDirectory, "Properties", SearchOption.AllDirectories);
            foreach (var item in dir)
            {
                Projects.Add(new Project(item));
            }
        }

        private DelegateCommand<ResXLocalizationFile> viewTranslationFile ;
        public DelegateCommand<ResXLocalizationFile> ViewTranslationFileCommand 
            => viewTranslationFile ??= viewTranslationFile = new DelegateCommand<ResXLocalizationFile>(ExecuteAddViewTranslationFileCommand);

        void ExecuteAddViewTranslationFileCommand(ResXLocalizationFile param)
        {
            CommonServiceLocator.ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<ViewTranslationFileEvent>()
                .Publish(param);
        }

        private DelegateCommand openResXGenerator;
        public DelegateCommand OpenResXGeneratorCommand =>
            openResXGenerator ??= openResXGenerator = new DelegateCommand(ExecuteOpenResXGenerator);

        void ExecuteOpenResXGenerator()
        {
            _viewService.ShowTranslateResXDialog();
        }

        private DelegateCommand openSolutionFolderCommand;
        public DelegateCommand OpenSolutionFolderCommand =>
            openSolutionFolderCommand ??= openSolutionFolderCommand = new DelegateCommand(ExecuteOpenSolutionFolderCommand);

        void ExecuteOpenSolutionFolderCommand()
        {
            var solutionDirectory = _viewService.BrowseFolder();

            if(string.IsNullOrEmpty(solutionDirectory))
            {
                return;
            }

            LoadSolution(solutionDirectory);
        }
    }

    public class ViewTranslationFileEvent : PubSubEvent<ResXLocalizationFile> { }
}
