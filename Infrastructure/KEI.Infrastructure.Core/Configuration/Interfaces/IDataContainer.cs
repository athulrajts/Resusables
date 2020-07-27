using System.Collections.Generic;
using System.ComponentModel;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure.Configuration
{
    public interface IDataContainer : INotifyPropertyChanged
    {
        IReadOnlyCollection<DataObject> DataCollection { get; }
        string FilePath { get; set; }
        string Name { get; set; }
        TypeInfo UnderlyingType { get; set; }
        bool ContainsProperty(string key);
        bool Get<T>(string key, ref T value);
        object Morph();
        T Morph<T>();
        object MorphList();
        List<T> MorphList<T>();
        void Set(string key, object value);
        bool Store();
        bool Store(string path);
        IEnumerator<DataObject> GetEnumerator();
        IEnumerable<string> GetKeys();

        bool Merge(IDataContainer data);
        
        void AddItem(DataObject obj);
        void RemoveItem(DataObject obj);
    }
}