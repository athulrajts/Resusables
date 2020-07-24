using CommonServiceLocator;
using KEI.Infrastructure;
using KEI.Infrastructure.Database;
using KEI.Infrastructure.Types;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Linq;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Database.Models;
using Application.Core;
using Application.Core.Interfaces;

namespace Application.UI.AdvancedSetup.ViewModels
{
    public class DatabaseSetupViewModel : BindableBase, IAdvancedSetup
    {
        public ObservableCollection<ResultViewModel> ResultTypes { get; set; } = new ObservableCollection<ResultViewModel>();
        public ObservableCollection<ResultProperty> SelectedResultProperties { get; set; } = new ObservableCollection<ResultProperty>();

        private readonly IViewService _viewService;
        private readonly IPropertyContainer _currentRecipe;
        private readonly DatabaseSetup setup = null;
        private readonly IPropertyContainer setupConfig = null;
        public DatabaseSetupViewModel()
        {
            _viewService = ServiceLocator.Current.GetInstance<IViewService>();
            _currentRecipe = ServiceLocator.Current.GetInstance<IEquipment>().CurrentRecipe;
            var appMode = ServiceLocator.Current.GetInstance<ISystemStatusManager>().ApplicationMode;

            _currentRecipe.Get($"{appMode} DB", ref setupConfig);

            setup = (DatabaseSetup)setupConfig.Morph();

            var resultTypes = ServiceLocator.Current.GetInstance<ImplementationsProvider>().GetImplementations(typeof(IDatabaseContext));
            resultTypes.ForEach(x =>
            {
                    var rm = new ResultViewModel(x);
                    foreach (ResultProperty item in rm.Children)
                    {
                        item.PropertyChanged += Selection_Changed;

                        if (setup?.Schema.FirstOrDefault(col => col.FullName == item.Column.FullName) is DatabaseColumn dc)
                        {
                            item.Column = dc;
                            item.IsSelected = true;
                        }
                    }
                    ResultTypes.Add(rm);
            });

            var passFailColumn = new ResultProperty { Column = new PassFailDatabaseColumn() };
            var errColumn = new ResultProperty { Column = new ErrorDatabaseColumn() };

            passFailColumn.PropertyChanged += Selection_Changed;
            errColumn.PropertyChanged += Selection_Changed;

            ResultTypes.Add(new ResultViewModel { Name = "Aggregates", Children = new ObservableCollection<object> { passFailColumn, errColumn } });

            if (setup?.Schema.FirstOrDefault(col => col.GetType() == typeof(ErrorDatabaseColumn)) is ErrorDatabaseColumn err)
                errColumn.IsSelected = true;

            if (setup?.Schema.FirstOrDefault(col => col.GetType() == typeof(PassFailDatabaseColumn)) is PassFailDatabaseColumn pfc)
                passFailColumn.IsSelected = true;
        }

        private DelegateCommand<ResultProperty> editCommand;
        public ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                    editCommand = new DelegateCommand<ResultProperty>((obj) => _viewService.EditObject(obj.Column));
                return editCommand;
            }
        }

        private DelegateCommand<ResultProperty> removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                    removeCommand = new DelegateCommand<ResultProperty>((obj) => obj.IsSelected = false);
                return removeCommand;
            }
        }

        private DelegateCommand saveSchemaCommand;
        public ICommand SaveSchemaCommand
        {
            get
            {
                if (saveSchemaCommand == null)
                    saveSchemaCommand = new DelegateCommand(() =>
                    {
                        setup.Schema = SelectedResultProperties.Select(x => x.Column).ToList();
                        setupConfig.Set("Schema", setup.Schema.ToListPropertyContainer("Schema"));
                        _currentRecipe.Store();

                    });
                return saveSchemaCommand;
            }
        }

        public string Name => "Database Setup";

        public bool IsAvailable => true;

        ~DatabaseSetupViewModel()
        {
            foreach (ResultViewModel item in ResultTypes)
            {
                foreach (ResultProperty props in item.Children)
                {
                    props.PropertyChanged -= Selection_Changed;
                }
            }
        }

        private void Selection_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ResultProperty rp)
            {
                if (e.PropertyName == nameof(ResultProperty.IsSelected))
                {
                    if (rp.IsSelected)
                    {
                        if (!SelectedResultProperties.Contains(rp))
                        {
                            SelectedResultProperties.Add(rp);
                        }
                    }
                    else
                    {
                        SelectedResultProperties.Remove(rp);
                    }
                }
            }
        }
    }

    public class ResultViewModel : TreeNode
    {
        public ResultViewModel(Type resultType)
        {
            var props = resultType.GetProperties();

            foreach (var prop in props)
            {
                Children.Add(new ResultProperty(prop));
            }

            Name = resultType.Name;

        }

        public ResultViewModel()
        {

        }
    }

    public class ResultProperty : BindableBase
    {
        public string Name => Column.Name;

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }

        public DatabaseColumn Column { get; set; }

        public ResultProperty(PropertyInfo propInfo)
        {
            if (propInfo.PropertyType == typeof(int) ||
               propInfo.PropertyType == typeof(float) ||
               propInfo.PropertyType == typeof(double))
            {
                Column = new NumericDatabaseColumn(propInfo);
            }
            else
            {
                Column = new DatabaseColumn(propInfo);
            }
        }

        public ResultProperty()
        {

        }

    }

    public class TreeNode : BindableBase
    {
        public ObservableCollection<object> Children { get; set; } = new ObservableCollection<object>();

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
    }

    public class DatabaseItem : IDatabaseContext
    {
        public double Property1 { get; set; }
        public double Property2 { get; set; }
        public double Property3 { get; set; }
        public double Property4 { get; set; }
        public double Property5 { get; set; }

        public static DatabaseItem GetRandom()
        {
            var rand = new Random();

            return new DatabaseItem
            {
                Property1 = rand.NextDouble() * 100,
                Property2 = rand.NextDouble() * 100,
                Property3 = rand.NextDouble() * 100,
                Property4 = rand.NextDouble() * 100,
                Property5 = rand.NextDouble() * 100,
            };
        }
    }
}
