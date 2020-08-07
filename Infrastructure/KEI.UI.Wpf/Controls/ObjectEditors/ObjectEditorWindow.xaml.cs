using KEI.Infrastructure.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace KEI.UI.Wpf.Controls.ObjectEditors
{
    /// <summary>
    /// Interaction logic for ObjectEditorWindow.xaml
    /// </summary>
    public partial class ObjectEditorWindow : Window
    {
        public ObjectEditorWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObjectEditorWindow(object editingObject)
        {
            InitializeComponent();
            DataContext = this;
            EditingCollection.Clear();
            if (editingObject is IPropertyContainer c)
            {
                if (c.UnderlyingType != null && typeof(IList).IsAssignableFrom(c.UnderlyingType.GetUnderlyingType()))
                {
                    IsCollection = true;

                    foreach (var item in c)
                    {
                        EditingCollection.Add(item.Value);
                    }
                }
                else
                {
                    IsCollection = false;
                    EditingObject = editingObject;
                    Width = 500;
                }
            }
            else if (editingObject is IList list)
            {
                originaList = editingObject as IList;
                IsCollection = true;

                var newTypes = new ObservableCollection<Type>();
                foreach (var item in originaList)
                {
                    if (!newTypes.Contains(item.GetType()))
                        newTypes.Add(item.GetType());

                    EditingCollection.Add(item);
                }

                if (newTypes.Count == 0)
                {
                    var type = originaList.GetType();
                    if (type.IsGenericType && !type.IsAbstract)
                    {
                        if (type.GenericTypeArguments.Length > 0)
                            newTypes.Add(type.GenericTypeArguments[0]);
                    }
                }

                NewItemTypes = newTypes;

                collectionControl.ItemAdded += ItemAdded;
                collectionControl.ItemRemoved += ItemRemoved;
                collectionControl.ItemMovedUp += ItemMovedUp;
                collectionControl.ItemMovedDown += ItemMovedDown;
            }
            else
            {
                IsCollection = false;
                EditingObject = editingObject;
                Width = 500;
            }
        }


        private IList originaList;
        public ObjectEditorWindow(IList editingCollection, List<Type> newItemTypes)
        {
            InitializeComponent();
            DataContext = this;

            originaList = editingCollection;
            IsCollection = true;

            EditingCollection.Clear();
            foreach (var item in editingCollection)
            {
                if (!newItemTypes.Contains(item.GetType()))
                    newItemTypes.Add(item.GetType());

                EditingCollection.Add(item);
            }

            NewItemTypes = new ObservableCollection<Type>(newItemTypes);

            collectionControl.ItemAdded += ItemAdded;
            collectionControl.ItemRemoved += ItemRemoved;
            collectionControl.ItemMovedUp += ItemMovedUp;
            collectionControl.ItemMovedDown += ItemMovedDown;

        }

        private void ItemMovedDown(object sender, object e)
        {
            int pos = originaList.IndexOf(e);
            if (pos < originaList.Count)
            {
                originaList.Remove(e);
                originaList.Insert(pos + 1, e);
            }
        }

        private void ItemMovedUp(object sender, object e)
        {
            int pos = originaList.IndexOf(e);
            if (pos > 0)
            {
                originaList.Remove(e);
                originaList.Insert(pos - 1, e);
            }
        }

        private void ItemRemoved(object sender, object e) => originaList.Remove(e);
        private void ItemAdded(object sender, object e) => originaList.Add(e);

        ~ObjectEditorWindow()
        {
            collectionControl.ItemAdded -= ItemAdded;
            collectionControl.ItemRemoved -= ItemRemoved;
        }

        public ObservableCollection<object> EditingCollection
        {
            get { return (ObservableCollection<object>)GetValue(EditingCollectionProperty); }
            set { SetValue(EditingCollectionProperty, value); }
        }

        public static readonly DependencyProperty EditingCollectionProperty =
            DependencyProperty.Register("EditingCollection", typeof(ObservableCollection<object>), typeof(ObjectEditorWindow), new PropertyMetadata(new ObservableCollection<object>()));

        public object EditingObject
        {
            get { return GetValue(EditingObjectProperty); }
            set { SetValue(EditingObjectProperty, value); }
        }

        public static readonly DependencyProperty EditingObjectProperty =
            DependencyProperty.Register("EditingObject", typeof(object), typeof(ObjectEditorWindow), new PropertyMetadata(null));

        public bool IsCollection
        {
            get { return (bool)GetValue(IsCollectionProperty); }
            set { SetValue(IsCollectionProperty, value); }
        }

        public static readonly DependencyProperty IsCollectionProperty =
            DependencyProperty.Register("IsCollection", typeof(bool), typeof(ObjectEditorWindow), new PropertyMetadata(false));



        public ObservableCollection<Type> NewItemTypes
        {
            get { return (ObservableCollection<Type>)GetValue(NewItemTypesProperty); }
            set { SetValue(NewItemTypesProperty, value); }
        }

        public static readonly DependencyProperty NewItemTypesProperty =
            DependencyProperty.Register("NewItemTypes", typeof(ObservableCollection<Type>), typeof(ObjectEditorWindow), new PropertyMetadata(null));


    }
}
