using Application.Core;
using Application.Core.Interfaces;
using KEI.Infrastructure;
using KEI.Infrastructure.Prism;
using Prism.Events;
using Prism.Mvvm;
using System.Data;

namespace Application.Engineering.Layout.Sections.ViewModels
{
    public class DatabaseSectionViewModel : BindableBase
    {
        private DataTable data;
        public DataTable Data
        {
            get { return data; }
            set { SetProperty(ref data, value); }
        }

        private readonly IDatabaseManager _databaseManager;
        public DatabaseSectionViewModel(IEventAggregator eventAggregator, IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;

            Data = _databaseManager[$"{ApplicationMode.Engineering} DB"].GetData();

            eventAggregator.GetEvent<TestExecuted>().Subscribe(result =>
            {
                Data = _databaseManager[$"{ApplicationMode.Engineering} DB"].GetData();
            }, 
            ThreadOption.BackgroundThread, false,
            (result) => result.Item1 == ApplicationMode.Engineering);

        }
    }
}
