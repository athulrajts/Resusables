﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using KEI.Infrastructure.Types;

namespace KEI.Infrastructure
{
    public interface IDataContainer : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public string FilePath { get; }
        public string Name { get; set; }
        public TypeInfo UnderlyingType { get; set; }
        public int Count { get; }
        
        public object this[string key] { get;set; }
        public bool GetValue<T>(string key, ref T value);
        public bool SetValue(string key, object value);
        public object Morph();
        public T Morph<T>();
        public IList MorphList();
        public IList<T> MorphList<T>();
        
        public bool Store();
        public bool Store(string path);
        
        public IEnumerator<DataObject> GetEnumerator();
        public IEnumerable<string> GetKeys();
        
        public DataObject Find(string key);
        public bool ContainsData(string key);
        public void Add(DataObject obj);
        public void Remove(DataObject obj);

        public bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression);
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        public bool SetBinding<T>(Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        public bool RemoveBinding<T>(Expression<Func<T>> expression);
    }
}