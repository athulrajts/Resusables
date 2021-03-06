﻿using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System.ComponentModel;
using System.Windows.Controls;

namespace Application.Engineering.Layout
{
    public class ContentSectionModel : BindableBase
    {
        private Dock dock;
        private bool isChecked;
        private double minWidth = double.NaN;
        private double minHeight = double.NaN;
        private double maxHeight = double.NaN;

        [Browsable(false)]
        public string Region { get; set; }

        public string Title { get; set; }

        [Browsable(false)]
        public string IconSource { get; set; }

        public Dock Dock
        {
            get { return dock; }
            set { SetProperty(ref dock, value); }
        }

        public double MinWidth
        {
            get { return minWidth; }
            set { SetProperty(ref minWidth, value); }
        }

        public double MinHeight
        {
            get { return minHeight; }
            set { SetProperty(ref minHeight, value); }
        }

        public double MaxHeight
        {
            get { return maxHeight; }
            set { SetProperty(ref maxHeight, value); }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value, OnCheckedChanged); }
        }

        private void OnCheckedChanged()
        {
            if (isChecked)
            {
                ContainerLocator.Container.Resolve<IEventAggregator>().GetEvent<SectionAdded>().Publish(this);
            }
            else
            {
                ContainerLocator.Container.Resolve<IEventAggregator>().GetEvent<SectionRemoved>().Publish(this);
            }
        }

    }
}
