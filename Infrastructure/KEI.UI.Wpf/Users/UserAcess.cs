using KEI.Infrastructure.UserManagement;
using Prism.Ioc;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KEI.UI.Wpf
{
    public class UserAcess
    {
        public static UserLevel GetEnabledForUserLevel(DependencyObject obj)
        {
            return (UserLevel)obj.GetValue(EnabledForUserLevelProperty);
        }

        public static void SetEnabledForUserLevel(DependencyObject obj, UserLevel value)
        {
            obj.SetValue(EnabledForUserLevelProperty, value);
        }

        public static readonly DependencyProperty EnabledForUserLevelProperty =
            DependencyProperty.RegisterAttached("EnabledForUserLevel", typeof(UserLevel), typeof(UserAcess), new PropertyMetadata(UserLevel.Operator, OnEnableForUserLevelChanged));

        private static void OnEnableForUserLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement u && e.NewValue is UserLevel l)
            {
                var binding = new Binding
                {
                    Source = ContainerLocator.Container.Resolve<IUserManager>(),
                    Path = new PropertyPath(nameof(IUserManager.CurrentUser)),
                    Mode = BindingMode.OneWay,
                    Converter = new UserLevelToEnabledConverter(),
                    ConverterParameter = l
                };

                BindingOperations.SetBinding(u, UIElement.IsEnabledProperty, binding);
            }
        }


        public static UserLevel GetVisibleForUserLevel(DependencyObject obj)
        {
            return (UserLevel)obj.GetValue(VisibleForUserLevelProperty);
        }

        public static void SetVisibleForUserLevel(DependencyObject obj, UserLevel value)
        {
            obj.SetValue(VisibleForUserLevelProperty, value);
        }

        public static readonly DependencyProperty VisibleForUserLevelProperty =
            DependencyProperty.RegisterAttached("VisibleForUserLevel", typeof(UserLevel), typeof(UserAcess), new PropertyMetadata(UserLevel.Operator, OnVisibleForUserLevelChanged));

        private static void OnVisibleForUserLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement u && e.NewValue is UserLevel l)
            {
                var binding = new Binding
                {
                    Source = ContainerLocator.Container.Resolve<IUserManager>(),
                    Path = new PropertyPath(nameof(IUserManager.CurrentUser)),
                    Mode = BindingMode.OneWay,
                    Converter = new UserLevelToVisibleConverter(),
                    ConverterParameter = l
                };

                BindingOperations.SetBinding(u, UIElement.VisibilityProperty, binding);
            }
        }
    }

    #region Converters

    class UserLevelToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IUser user && parameter is UserLevel level)
            {
                if ((int)user.Level < (int)level)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class UserLevelToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IUser user && parameter is UserLevel level)
            {
                if ((int)user.Level < (int)level)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
