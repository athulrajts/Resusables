﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigEditor.Views
{
    /// <summary>
    /// Interaction logic for ConfigViewer.xaml
    /// </summary>
    public partial class ConfigViewer : UserControl
    {
        public ConfigViewer()
        {
            InitializeComponent();
        }

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            (treeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsSelected = true;
        }
    }
}
