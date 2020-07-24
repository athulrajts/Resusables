using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Application.Core.Interfaces
{
    public interface IEquipment : INotifyPropertyChanged
    {
        event EventHandler<IPropertyContainer> RecipeLoaded;
        IPropertyContainer CurrentRecipe { get; }
        IDataContainer CurrentDatabaseSetup { get; }
        void LoadRecipe(string path);
        void StoreRecipe(string path);
        void RestoreDefaultRecipe();
        void ExecuteTest();
    }

    public class TestExecuted : PubSubEvent<Tuple<ApplicationMode, string, List<TestResult>>> { }
}
