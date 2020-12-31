using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace KEI.Infrastructure
{
    /// <summary>
    /// base class for wrapping another class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Wrapper<T> : BindableObject
    {
        public T Content { get; set; }

        public Wrapper(T content)
        {
            Content = content;

            if(Content is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += OnContentPropertyChanged;
            }
        }

        ~Wrapper()
        {
            if (Content is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= OnContentPropertyChanged;
            }
        }

        public TReturn InvokeMethod<TReturn>(Func<T,TReturn> methodDelegate)
        {
            if(methodDelegate is null)
            {
                throw new ArgumentNullException();
            }

            return methodDelegate(Content);
        }

        public TReturn GetProperty<TReturn>(Func<T, TReturn> propertyAccessor)
        {
            if(propertyAccessor is null)
            {
                throw new ArgumentNullException();
            }

            return propertyAccessor(Content);
        }

        public void SetProperty<TProperty>(Expression<Func<T, TProperty>> propertyAccessor, TProperty value)
        {
            var memberExpression = (MemberExpression)propertyAccessor.Body;

            if (memberExpression is null)
            {
                throw new ArgumentException("propertyAccessor should be a Member expression");
            }

            var property = (PropertyInfo)memberExpression.Member;
            var setMethod = property.GetSetMethod();

            var parameterT = Expression.Parameter(typeof(T), "x");
            var parameterTProperty = Expression.Parameter(typeof(TProperty), "y");

            var newExpression =
                Expression.Lambda<Action<T, TProperty>>(
                    Expression.Call(parameterT, setMethod, parameterTProperty),
                    parameterT,
                    parameterTProperty
                );

            newExpression.Compile().Invoke(Content, value);
        }

        public static implicit operator T(Wrapper<T> result) => result.Content;
        public static implicit operator Wrapper<T>(T result) => new Wrapper<T>(result);

        protected virtual void OnContentPropertyChanged(object sender, PropertyChangedEventArgs e) 
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
