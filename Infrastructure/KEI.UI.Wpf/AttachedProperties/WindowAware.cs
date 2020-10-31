using System.Windows;
using KEI.Infrastructure;

namespace KEI.UI.Wpf.AttachedProperties
{
    public static class WindowAware
    {
        public static bool GetIsClosable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsClosableProperty);
        }

        public static void SetIsClosable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsClosableProperty, value);
        }

        public static readonly DependencyProperty IsClosableProperty =
            DependencyProperty.RegisterAttached("IsClosable", typeof(bool), typeof(WindowAware), new PropertyMetadata(false, OnClosableValueChanged));

        private static void OnClosableValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window w && e.NewValue is bool isClosable)
            {
                if (w.DataContext is IClosable closable && isClosable)
                {
                    closable.Close += () => w.Close();
                }
            }
        }

        public static bool GetIsMaximizable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMaximizableProperty);
        }

        public static void SetIsMaximizable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMaximizableProperty, value);
        }

        public static readonly DependencyProperty IsMaximizableProperty =
            DependencyProperty.RegisterAttached("IsMaximizable", typeof(bool), typeof(WindowAware), new PropertyMetadata(false, OnMaximizableChanged));

        private static void OnMaximizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window w && e.NewValue is bool isMaximizable)
            {
                if (w.DataContext is IMaximizable maximizable && isMaximizable)
                {
                    maximizable.Maximize += () => w.WindowState = WindowState.Maximized;
                    maximizable.RestoreDown += () => w.WindowState = WindowState.Normal;
                }
            }
        }

        public static bool GetIsMinimizable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMinimizableProperty);
        }

        public static void SetIsMinimizable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMinimizableProperty, value);
        }

        public static readonly DependencyProperty IsMinimizableProperty =
            DependencyProperty.RegisterAttached("IsMinimizable", typeof(bool), typeof(WindowAware), new PropertyMetadata(false, OnMinimizableChanged));

        private static void OnMinimizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window w && e.NewValue is bool isMinimizable)
            {
                if (w.DataContext is IMinimizable minimizable && isMinimizable)
                {
                    minimizable.Minimize += () => w.WindowState = WindowState.Minimized;
                }
            }
        }

        public static bool GetIsWindowAware(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWindowAwareProperty);
        }

        public static void SetIsWindowAware(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWindowAwareProperty, value);
        }

        public static readonly DependencyProperty IsWindowAwareProperty =
            DependencyProperty.RegisterAttached("IsWindowAware", typeof(bool), typeof(WindowAware), new PropertyMetadata(false, OnWindowAwareChanged));

        private static void OnWindowAwareChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isWindowAware)
            {
                SetIsClosable(d, isWindowAware);
                SetIsMaximizable(d, isWindowAware);
                SetIsMinimizable(d, isWindowAware);
            }

        }
    }
}
