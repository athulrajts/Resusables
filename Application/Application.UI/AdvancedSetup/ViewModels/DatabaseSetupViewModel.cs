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
using System.Collections.Specialized;

namespace Application.UI.AdvancedSetup.ViewModels
{

    public class DatabaseSetupViewModel : BindableBase, IAdvancedSetup
    {
        public ObservableCollection<ResultViewModel> ResultTypes { get; set; } = new ObservableCollection<ResultViewModel>();
        public ObservableCollection<ResultProperty> SelectedResultProperties { get; set; } = new ObservableCollection<ResultProperty>();

        private readonly IViewService _viewService;
        private readonly IDataContainer _currentRecipe;
        private readonly DatabaseSetup _setup;
        private readonly IDataContainer _setupConfig;
        
        public DatabaseSetupViewModel(IEquipment equipment, IViewService viewService,
            ISystemStatusManager systemStatusManager, ImplementationsProvider provider)
        {
            _viewService = viewService;
            _currentRecipe = equipment.CurrentRecipe;

            ResultTypes.CollectionChanged += ResultTypes_CollectionChanged;

            var appMode = systemStatusManager.ApplicationMode;
            _currentRecipe.Get($"{appMode} DB", ref _setupConfig);
            _setup = (DatabaseSetup)_setupConfig.Morph();

            // Take currently valid results
            PopulateSetup(provider.GetImplementations<IDatabaseContext>().ToArray());

            AddPassFailColumnTypes();
        }

        /// <summary>
        /// For Demo purpose only
        /// Actual implementation should not use this constructure
        /// </summary>
        /// <param name="setupConfig"></param>
        /// <param name="resultTypes"></param>
        public DatabaseSetupViewModel(IViewService viewService, IDataContainer setupConfig, params Type[] resultTypes)
        {
            _viewService = viewService;
            _setupConfig = setupConfig;
            _setup = setupConfig.Morph<DatabaseSetup>();

            ResultTypes.CollectionChanged += ResultTypes_CollectionChanged;

            // Show user given results only
            PopulateSetup(resultTypes);

            AddPassFailColumnTypes();
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
                        _setup.Schema = SelectedResultProperties.Select(x => x.Column).ToList();
                        _setupConfig.Set("Schema", _setup.Schema.ToListPropertyContainer("Schema"));
                        _currentRecipe?.Store();

                    });
                return saveSchemaCommand;
            }
        }

        public string Name => "Database Setup";

        public bool IsAvailable => true;

        ~DatabaseSetupViewModel()
        {
            ResultTypes.CollectionChanged -= ResultTypes_CollectionChanged;
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
                        if (SelectedResultProperties.Contains(rp) == false)
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

        private void ResultTypes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var result in e.NewItems.Cast<ResultViewModel>())
                {
                    foreach (var property in result.Children.Cast<ResultProperty>())
                    {
                        property.PropertyChanged += Selection_Changed;

                        if (_setup?.Schema.FirstOrDefault(col => col.FullName == property.Column.FullName) is DatabaseColumn dc)
                        {
                            property.Column = dc;
                            property.IsSelected = true;
                        }
                    }
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var result in e.OldItems.Cast<ResultViewModel>())
                {
                    foreach (var property in result.Children.Cast<INotifyPropertyChanged>())
                    {
                        property.PropertyChanged -= Selection_Changed;
                    }
                }
            }
        }

        private void PopulateSetup(params Type[] resultTypes) => 
            resultTypes.ToList()
            .ForEach(x => ResultTypes.Add(new ResultViewModel(x)));

        private void AddPassFailColumnTypes()
        {
            var passFailColumn = new ResultProperty { Column = new PassFailDatabaseColumn() };
            var errColumn = new ResultProperty { Column = new ErrorDatabaseColumn() };

            passFailColumn.PropertyChanged += Selection_Changed;
            errColumn.PropertyChanged += Selection_Changed;

            ResultTypes.Add(new ResultViewModel 
            {
                Name = "Aggregates",
                Children = new ObservableCollection<object> { passFailColumn, errColumn }
            });

            errColumn.IsSelected = _setup?
                .Schema
                .FirstOrDefault(col => col.GetType() == typeof(ErrorDatabaseColumn)) is ErrorDatabaseColumn;

            passFailColumn.IsSelected = _setup?
                .Schema
                .FirstOrDefault(col => col.GetType() == typeof(PassFailDatabaseColumn)) is PassFailDatabaseColumn;
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
