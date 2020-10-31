using System.Windows.Input;
using System.Threading.Tasks;
using Prism.Commands;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using KEI.Infrastructure.Screen;
using KEI.Infrastructure.Validation;
using KEI.Infrastructure.Configuration;
using KEI.UI.Wpf;
using KEI.UI.Wpf.Hotkey;
using Application.Core;
using Application.Core.Constants;
using Application.Production.Views;
using System;

namespace Application.Production.ViewModels
{
    [Screen(DisplayName = "Demos",
             Icon = Icon.ShowAllCode16x,
             ScreenName = nameof(DemoScreen))]
    public class DemoScreenViewModel : BaseScreenViewModel<DemoScreen>
    {
        public override string DisplayName { get; set; } = "Demos";
        public override Icon Icon { get; set; } = Icon.ShowAllCode16x;

        private readonly IViewService _viewService;
        private readonly ISystemStatusManager _statusManager;
        public DemoScreenViewModel(IHotkeyService hotkeyService, IViewService viewService,
            ISystemStatusManager statusManager, IConfigManager configManager) : base(hotkeyService)
        {
            _viewService = viewService;
            _statusManager = statusManager;

            var generalPerferences = configManager.GetConfig(ConfigKeys.GeneralPreferences);

            SamplePropertyContainer = PropertyContainerBuilder.Create("Config").WithProperty("PropertyOne", true).Build();

            //IPropertyContainer samplePropertyContainerThree = new PropertyDictionary
            //{
            //    {"IntegerProperty", 22 },
            //    {"FloalProperty", 3.14},
            //    {"DoubleProperty", 3.14142 },
            //    {"BoolProperty", false },
            //    {"StringProperty", "Hello World" },
            //    {"EnumProperty", ApplicationMode.Production }
            //};

            ValidationRule = ValidationBuilder.Create(true)
                .Length(5)
                .Length(2, 10)
                .MaxLength(7)
                .MinLength(3)
                .NotNullOrEmpty()
                .Directory()
                .File()
                .Range(10, 100)
                .OutsideRange(10, 100)
                .Positive()
                .Negative()
                .Type(typeof(int))
                .Type(typeof(double))
                .Type(typeof(float))
                .Type(typeof(bool));

            SamplePropertyContainer.SetBinding("PropertyOne", () => OneTimeBinding, BindingMode.OneTime);
            SamplePropertyContainer.SetBinding("PropertyOne", () => OneWayBinding, BindingMode.OneWay);
            SamplePropertyContainer.SetBinding("PropertyOne", () => TwoWayBinding, BindingMode.TwoWay);
            SamplePropertyContainer.SetBinding("PropertyOne", () => OneWayToSourceBinding, BindingMode.OneWayToSource);

            generalPerferences.SetBinding("Theme", () => Theme, BindingMode.TwoWay);

            ValidationRule.Validate(ValidatingText);
            
            InitCommands();

            RaisePropertyChanged(string.Empty);
        }

        private void UpdateValidation()
        {
            ValidationRule.Validate(ValidatingText);
            RaisePropertyChanged(nameof(ValidationRule));
        }
        private void InitCommands()
        {
            ShowInfoCommand = new DelegateCommand(() => _viewService.Inform(DialogText));
            ShowWarningCommand = new DelegateCommand(() => _viewService.Warn(DialogText));
            ShowErrorCommand = new DelegateCommand(() => _viewService.Error(DialogText));
            SetBusyCommand = new DelegateCommand(async () =>
            {
                _viewService.SetBusy("Loading", "5");
                for (int i = 5; i > 0; i--)
                {
                    await Task.Delay(1000);
                    _viewService.UpdateBusyText("Loading", $"{i - 1}");
                }
                _viewService.SetAvailable();
            });
           
            ShowPromptCommand = new DelegateCommand<object>(option =>
            {
                if (option is PromptOptions po)
                {
                    _viewService.PromptWithDefault(DialogText, po, PromptResult.Retry, TimeSpan.FromSeconds(5));
                }
            });
        }

        private string dialogText = "Hello World!!";
        public string DialogText
        {
            get { return dialogText; }
            set { SetProperty(ref dialogText, value); }
        }

        private Theme theme;
        public Theme Theme
        {
            get { return theme; }
            set { SetProperty(ref theme, value); }
        }
        private Language lanugage;
        public Language Language
        {
            get { return lanugage; }
            set { SetProperty(ref lanugage, value, () => _statusManager.CurrentLanguage = Language); }
        }

        private bool oneWayBinding;
        public bool OneWayBinding
        {
            get { return oneWayBinding; }
            set { SetProperty(ref oneWayBinding, value); }
        }

        private bool twoWayBinding;
        public bool TwoWayBinding
        {
            get { return twoWayBinding; }
            set { SetProperty(ref twoWayBinding, value); }
        }

        private bool oneTimeBinding;
        public bool OneTimeBinding
        {
            get { return oneTimeBinding; }
            set { SetProperty(ref oneTimeBinding, value); }
        }

        private bool oneWayToSourceBinding;
        public bool OneWayToSourceBinding
        {
            get { return oneWayToSourceBinding; }
            set { SetProperty(ref oneWayToSourceBinding, value); }
        }

        private string validatingText = "Hello World!!";
        public string ValidatingText
        {
            get { return validatingText; }
            set { SetProperty(ref validatingText, value, () => UpdateValidation()); }
        }

        public ValidatorGroup ValidationRule { get; set; }
        public ICommand ShowInfoCommand { get; set; }
        public ICommand ShowWarningCommand { get; set; }
        public ICommand ShowErrorCommand { get; set; }
        public ICommand SetBusyCommand { get; set; }
        public ICommand ShowPromptCommand { get; set; }
        public ICommand EditDatabaseCommand { get; set; }
        public ICommand LogNowCommand { get; set; }
        public IPropertyContainer SamplePropertyContainer { get; set; }
    }
}
