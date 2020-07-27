using System;
using System.Linq.Expressions;

namespace KEI.Infrastructure.Configuration
{
    public interface IPropertyContainer : IDataContainer, ICloneable
    {
        public bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression);
        public bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        public bool SetBrowseOptions(string property, BrowseOptions option);
    }
}