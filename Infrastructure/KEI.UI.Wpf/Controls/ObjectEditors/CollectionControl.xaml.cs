using KEI.Infrastructure.Validation;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KEI.UI.Wpf.Controls.Configuration
{
    /// <summary>
    /// Interaction logic for CollectionControl.xaml
    /// </summary>
    public partial class CollectionControl : UserControl
    {
        public ObservableCollection<object> ItemSource
        {
            get { return (ObservableCollection<object>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableCollection<object>), typeof(CollectionControl), new PropertyMetadata(new ObservableCollection<object>(), OnItemSourceChanged));

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CollectionControl;

            if (e.NewValue is ObservableCollection<object> c)
            {
                foreach (var item in c)
                {
                    cc.CollectionItems.Add(new ValidatingObject(item));
                }
            }

            cc.listView.SelectedIndex = 0;
        }

        public ObservableCollection<ValidatingObject> CollectionItems { get; set; } = new ObservableCollection<ValidatingObject>();

        public ObservableCollection<Type> NewItemTypes
        {
            get { return (ObservableCollection<Type>)GetValue(NewItemTypesProperty); }
            set { SetValue(NewItemTypesProperty, value); }
        }

        public static readonly DependencyProperty NewItemTypesProperty =
            DependencyProperty.Register("NewItemTypes", typeof(ObservableCollection<Type>), typeof(CollectionControl), new PropertyMetadata(null, OnNewTypesChanged));

        private static void OnNewTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cc = d as CollectionControl;

            if (e.NewValue is ObservableCollection<Type> types && types.Count > 0)
            {
                cc.newTypesCB.SelectedIndex = 0;
            }
        }

        public EventHandler<object> ItemAdded;
        public EventHandler<object> ItemRemoved;
        public EventHandler<object> ItemMovedUp;
        public EventHandler<object> ItemMovedDown;

        public CollectionControl()
        {
            InitializeComponent();
            newTypesCB.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            listView.ItemsSource = CollectionItems;

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (newTypesCB.SelectedItem == null)
                return;

            var instance = Activator.CreateInstance((Type)newTypesCB.SelectedItem);
            ItemSource.Add(instance);
            CollectionItems.Add(new ValidatingObject(instance));

            listView.SelectedItem = instance;
            ItemAdded?.Invoke(this, instance);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is ValidatingObject vo)
            {
                var pos = CollectionItems.IndexOf(vo);
                CollectionItems.Remove(vo);
                if (pos > 0)
                {
                    ItemSource.RemoveAt(pos); 
                }

                ItemRemoved?.Invoke(this, vo.SelectedObject);

                if (pos < CollectionItems.Count)
                    listView.SelectedIndex = pos;
                else
                    listView.SelectedIndex = CollectionItems.Count - 1;
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is ValidatingObject o)
            {
                int pos = CollectionItems.IndexOf(o);
                if (pos > 0)
                {
                    CollectionItems.Remove(o);
                    ItemSource.RemoveAt(pos);

                    CollectionItems.Insert(pos - 1, o);
                    ItemSource.Insert(pos - 1, o.SelectedObject);


                    listView.SelectedItem = o;
                    ItemMovedUp?.Invoke(this, o.SelectedObject);
                }
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is ValidatingObject o)
            {
                int pos = CollectionItems.IndexOf(o);
                if (pos < CollectionItems.Count)
                {
                    CollectionItems.Remove(o);
                    ItemSource.RemoveAt(pos);

                    CollectionItems.Insert(pos + 1, o);
                    ItemSource.Insert(pos + 1, o.SelectedObject);

                    listView.SelectedItem = o;
                    ItemMovedDown?.Invoke(this, o.SelectedObject);
                }
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is ValidatingObject o)
            {
                ValidatingObject copy = new ValidatingObject(Activator.CreateInstance(o.SelectedObject.GetType()));
                var props = o.SelectedObject.GetType().GetProperties();

                foreach (var prop in props)
                {
                    if (prop.SetMethod != null)
                    {
                        prop.SetValue(copy.SelectedObject, prop.GetValue(o.SelectedObject)); 
                    }
                }

                CollectionItems.Add(copy);
                ItemSource.Add(copy.SelectedObject);

                listView.SelectedItem = copy;

            }
        }

        
    }

    public class ValidatingObject : BindableBase
    {
        private object selectedObject;
        public object SelectedObject
        {
            get { return selectedObject; }
            set { SetProperty(ref selectedObject, value); }
        }

        private bool isValid = true;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }

        public ValidatingObject(object obj)
        {
            if (obj is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += Inpc_PropertyChanged;
            }
            SelectedObject = obj;
        }

        private void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsValid = Validators.IsValid(sender);
        }
    }
}
