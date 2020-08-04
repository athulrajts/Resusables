using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KEI.UI.Wpf.ViewService
{
    /// <summary>
    /// Interaction logic for GenericDialog.xaml
    /// </summary>
    public partial class GenericDialog : DialogWindow
    {
        //private DoubleAnimation shrinkOutAnimationX = new DoubleAnimation
        //{
        //    From = 1,
        //    To = 0,
        //    Duration = TimeSpan.FromMilliseconds(200)
        //};
        //private DoubleAnimation shrinkOutAnimationY = new DoubleAnimation
        //{
        //    From = 1,
        //    To = 0,
        //    Duration = TimeSpan.FromMilliseconds(200)
        //};
        //private Storyboard storyboard = new Storyboard();

        public GenericDialog()
        {
            InitializeComponent();

            //Storyboard.SetTargetProperty(shrinkOutAnimationX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            //Storyboard.SetTargetProperty(shrinkOutAnimationY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            //storyboard.Children.Add(shrinkOutAnimationX);
            //storyboard.Children.Add(shrinkOutAnimationY);
            //Storyboard.SetTarget(shrinkOutAnimationX, this);
            //Storyboard.SetTarget(shrinkOutAnimationY, this);

            DataContext = new GenericDialogViewModel();
        }
    }
}
