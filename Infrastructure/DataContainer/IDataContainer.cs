using System;
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
        /// <summary>
        /// Will contain the file path from which this instance was loaded
        /// will be empty if this instance was created dynamically in code.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Name of of this instance, can be empty
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represent the Object type this instance can be safely morphed into.
        /// it should contain all the public writable properties of the type
        /// </summary>
        public TypeInfo UnderlyingType { get; set; }

        /// <summary>
        /// Number of properties in this instance
        /// </summary>
        public int Count { get; }
        
        /// <summary>
        /// Get value by key
        /// Will throw <see cref="KeyNotFoundException"/> of <paramref name="key"/> does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns>value</returns>
        public object this[string key] { get;set; }
        
        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetValue<T>(string key, ref T value);
        
        /// <summary>
        /// Set value by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, object value);
        
        /// <summary>
        /// Create instance of type <see cref="UnderlyingType"/> if it's null
        /// throws exception
        /// </summary>
        /// <returns></returns>
        public object Morph();
        
        /// <summary>
        /// Forces to morph to give type even if <see cref="UnderlyingType"/> is not set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Morph<T>();
        
        // TODO : Does this still work ??
        // Is this still needed ??
        public IList MorphList();
        public IList<T> MorphList<T>();
        //
        
        /// <summary>
        /// Try to save to <see cref="FilePath"/>
        /// </summary>
        /// <returns></returns>
        public bool Store();

        /// <summary>
        /// Try to save to <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Store(string path);
        
        public IEnumerator<DataObject> GetEnumerator();
        
        /// <summary>
        /// Get all top level keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys();
        
        /// <summary>
        /// Find instance of <see cref="DataObject"/> corresponding to key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DataObject Find(string key);

        /// <summary>
        /// Check whether <paramref name="key"/> is present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsData(string key);

        /// <summary>
        /// Adds <see cref="DataObject"/> instance
        /// </summary>
        /// <param name="obj"></param>
        public void Add(DataObject obj);

        /// <summary>
        /// Removes <see cref="DataObject"/> instance
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(DataObject obj);

        /// <summary>
        /// Clear all properties
        /// </summary>
        public void Clear();

        /// <summary>
        /// Sets a binding between Property in external class and Property inside this instance
        /// Assumes that name of property in IDataContainer is same as the property in external class
        /// External class needs to implement <see cref="INotifyPropertyChanged"/> and fire <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event approprialy for this to work.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SetBinding<T>(Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);

        /// <summary>
        /// Same as <see cref="SetBinding{T}(Expression{Func{T}}, BindingMode)"/> but name of property in IDataContainer can be
        /// specified explicitly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyKey"></param>
        /// <param name="expression"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);

        /// <summary>
        /// Removes bindings set using <see cref="SetBinding{T}(Expression{Func{T}}, BindingMode)"/>
        /// or <see cref="SetBinding{T}(string, Expression{Func{T}}, BindingMode)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool RemoveBinding<T>(Expression<Func<T>> expression);

        /// <summary>
        /// Removes bindings set using <see cref="SetBinding{T}(Expression{Func{T}}, BindingMode)"/>
        /// or <see cref="SetBinding{T}(string, Expression{Func{T}}, BindingMode)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyKey"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression);

    }
}