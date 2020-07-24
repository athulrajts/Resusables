using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Windows;

namespace KEI.UI.Wpf.Controls.Configuration
{
    /// <summary>
    /// Interaction logic for CollectionEditor.xaml
    /// </summary>
    public partial class ValidationEditor : Window
    {
        public ValidationEditor(DataItem data, List<Type> newTypes = null)
        {
            InitializeComponent();
            DataContext = this;

            NewItemTypes = newTypes;
            if (data.ValidationRule is ValidatorGroup g)
            {
                ItemSource = g.Rules;
            }
        }



        public List<Type> NewItemTypes
        {
            get { return (List<Type>)GetValue(NewItemTypesProperty); }
            set { SetValue(NewItemTypesProperty, value); }
        }

        public static readonly DependencyProperty NewItemTypesProperty =
            DependencyProperty.Register("NewItemTypes", typeof(List<Type>), typeof(ValidationEditor), new PropertyMetadata(null));


        public List<ValidationRule> ItemSource
        {
            get { return (List<ValidationRule>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(List<ValidationRule>), typeof(ValidationEditor), new PropertyMetadata(null));

        private void CollectionControl_ItemAdded(object sender, Xceed.Wpf.Toolkit.ItemEventArgs e)
        {
            ItemSource.Add(e.Item as ValidationRule);
        }

        private void CollectionControl_ItemDeleted(object sender, Xceed.Wpf.Toolkit.ItemEventArgs e)
        {
            ItemSource.Remove(e.Item as ValidationRule);
        }
    }
}
