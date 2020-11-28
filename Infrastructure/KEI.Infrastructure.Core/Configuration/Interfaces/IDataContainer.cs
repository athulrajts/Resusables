using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    public interface IDataContainer : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public TypeInfo UnderlyingType { get; set; }
        public int Count { get; }
        
        public bool GetValue<T>(string key, ref T value);
        public void SetValue(string key, object value);
        public object Morph();
        public T Morph<T>();
        public IList MorphList();
        public IList<T> MorphList<T>();
        
        public bool Store();
        public bool Store(string path);
        
        public IEnumerator<DataObject> GetEnumerator();
        public IEnumerable<string> GetKeys();
        
        public DataObject Find(string key);
        public bool ContainsProperty(string key);
        public bool Merge(IDataContainer data);
        public void Add(DataObject obj);
        public void Remove(DataObject obj);
        
        public bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression);
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        public bool SetBinding<T>(Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        public bool RemoveBinding<T>(Expression<Func<T>> expression);
    }
}