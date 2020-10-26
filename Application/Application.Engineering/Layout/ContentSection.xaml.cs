using KEI.Infrastructure;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Application.Engineering.Layout
{
    /// <summary>
    /// Interaction logic for ContentSection.xaml
    /// </summary>
    public partial class ContentSection : UserControl
    {
        Storyboard closeSB = new Storyboard();
        DoubleAnimation closeAnimationX = new DoubleAnimation();
        DoubleAnimation closeAnimationY = new DoubleAnimation();
        DoubleAnimation fadeOutAnimation = new DoubleAnimation();
        Storyboard openSB = new Storyboard();
        DoubleAnimation OpenAnimationX = new DoubleAnimation();
        DoubleAnimation OpenAnimationY = new DoubleAnimation();
        DoubleAnimation fadeInAnimation = new DoubleAnimation();


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ContentSection), new PropertyMetadata("Title"));


        public string Region
        {
            get { return (string)GetValue(RegionProperty); }
            set { SetValue(RegionProperty, value); }
        }

        public static readonly DependencyProperty RegionProperty =
            DependencyProperty.Register("Region", typeof(string), typeof(ContentSection), new PropertyMetadata(""));


        public ContentSection()
        {
            InitializeComponent();

            var duration = TimeSpan.FromSeconds(0.3);

            closeAnimationX.Duration = new Duration(duration);
            closeAnimationX.From = 1;
            closeAnimationX.To = 0;
            closeAnimationY.Duration = new Duration(duration);
            closeAnimationY.From = 1;
            closeAnimationY.To = 0;
            fadeOutAnimation.Duration = new Duration(duration);
            fadeOutAnimation.From = 1;
            fadeOutAnimation.To = 0;
            Storyboard.SetTargetProperty(closeAnimationX, new PropertyPath("(RenderTransform).ScaleX"));
            Storyboard.SetTargetProperty(closeAnimationY, new PropertyPath("(RenderTransform).ScaleY"));
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            closeSB.Children.Add(closeAnimationX);
            closeSB.Children.Add(closeAnimationY);
            closeSB.Children.Add(fadeOutAnimation);
            Storyboard.SetTarget(closeAnimationX, this);
            Storyboard.SetTarget(closeAnimationY, this);
            Storyboard.SetTarget(fadeOutAnimation, this);


            OpenAnimationX.Duration = new Duration(duration);
            OpenAnimationX.From = 0;
            OpenAnimationX.To = 1;
            OpenAnimationY.Duration = new Duration(duration);
            OpenAnimationY.From = 0;
            OpenAnimationY.To = 1;
            fadeInAnimation.Duration = new Duration(duration);
            fadeInAnimation.From = 0;
            fadeInAnimation.To = 1;
            Storyboard.SetTargetProperty(OpenAnimationX, new PropertyPath("(RenderTransform).ScaleX"));
            Storyboard.SetTargetProperty(OpenAnimationY, new PropertyPath("(RenderTransform).ScaleY"));
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            openSB.Children.Add(OpenAnimationX);
            openSB.Children.Add(OpenAnimationY);
            openSB.Children.Add(fadeInAnimation);
            Storyboard.SetTarget(OpenAnimationX, this);
            Storyboard.SetTarget(OpenAnimationY, this);
            Storyboard.SetTarget(fadeInAnimation, this);
        }

        public async Task ShrinkAndFadeOut()
        {
            closeSB.Begin();

            await Task.Delay(300);

            Visibility = Visibility.Collapsed;
        }

        public async Task GrowAndFadeIn()
        {
            Visibility = Visibility.Visible;

            openSB.Begin();

            await Task.Delay(300);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            ContainerLocator.Container.Resolve<IViewService>().EditObject(DataContext);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ContentSectionModel csm)
            {
                csm.IsChecked = false;
            }
        }

        private void Expand_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ContentSectionModel csm)
            {
                ContainerLocator.Container.Resolve<IEventAggregator>().GetEvent<SectionExpanded>().Publish(csm);
            }
        }

        private void Minimized_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ContentSectionModel csm)
            {
                ContainerLocator.Container.Resolve<IEventAggregator>().GetEvent<SectionMinimized>().Publish(csm);
            }
        }

        private void Popout_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ContentSectionModel csm)
            {
                var window = new Window { Title = csm.Title };
                var content = new ContentControl();
                RegionManager.SetRegionName(content, csm.Region);
                window.Content = content;
                window.Closing += (_, __) => { csm.IsChecked = true; };
                csm.IsChecked = false;
                window.Show();
            }
        }
    }

    public class SectionAdded : PubSubEvent<ContentSectionModel> { }
    public class SectionRemoved : PubSubEvent<ContentSectionModel> { }
    public class SectionExpanded : PubSubEvent<ContentSectionModel> { }
    public class SectionMinimized : PubSubEvent<ContentSectionModel> { }
}
