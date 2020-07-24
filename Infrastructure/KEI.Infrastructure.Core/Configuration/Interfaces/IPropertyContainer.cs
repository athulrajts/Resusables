using System;
using System.Linq.Expressions;

namespace KEI.Infrastructure.Configuration
{
    public interface IPropertyContainer : IDataContainer, ICloneable
    {
        bool RemoveBinding<T>(string propertyKey, Expression<Func<T>> expression);
        bool SetBinding<T>(string propertyKey, Expression<Func<T>> expression, BindingMode mode = BindingMode.TwoWay);
        bool SetBrowseOptions(string property, BrowseOptions option);
    }
}