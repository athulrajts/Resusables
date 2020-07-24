﻿using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Windows;

namespace KEI.Infrastructure.Screen
{
    public abstract class BaseViewModel : BindableBase, IWeakEventListener
    {
        public virtual string ScreenName { get; }

        public BaseViewModel()
        {

        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                var eArgs = e as PropertyChangedEventArgs;

                ProcessPropertyChanged(sender, eArgs.PropertyName);

                return true;
            }
            return false;
        }

        protected virtual void ProcessPropertyChanged(object sender, string property)
        {
            return;
        }

        protected void AddPropertyObserver(INotifyPropertyChanged source, string property = "") => PropertyChangedEventManager.AddListener(source, this, property);

        protected void RemovePropertyObserver(INotifyPropertyChanged source, string property = "") => PropertyChangedEventManager.RemoveListener(source, this, property);
    }

    public abstract class BaseViewModel<TView> : BaseViewModel
    {
        public override string ScreenName => typeof(TView).Name;
    }
}
