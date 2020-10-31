using KEI.UI.Wpf.ViewService;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace KEI.UI.Wpf
{
    public class DialogWindow : UserControl
    {

        public bool Animate { get; set; } = true;
        public bool WindowLess { get; set; } = true;

        protected readonly Storyboard storyboard = new Storyboard();

        public DialogWindow()
        {
            DataContextChanged += DialogWindow_DataContextChanged;
            Loaded += DialogWindow_Loaded;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (WindowLess)
            {
                var resourceDictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/KEI.UI.Wpf;component/Themes/Windows.xaml") };
                Dialog.SetWindowStyle(this, resourceDictionary["FramelessWindow"] as Style);
            }

            if (Animate)
            {
                RenderTransform = new ScaleTransform(0, 0);
                RenderTransformOrigin = new Point(0.5, 0.5);
            }

            base.OnInitialized(e);
        }

        private async void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(Animate == false)
            {
                return;
            }

            var keyFrames = new DoubleKeyFrameCollection
            {
                new SplineDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(0),
                    Value = 0
                },
                new SplineDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(200),
                    Value = 1.2
                },
                new SplineDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(300),
                    Value = 0.9,
                },
                new SplineDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(400),
                    Value = 1
                }
            };

            var animationScaleX = new DoubleAnimationUsingKeyFrames { KeyFrames = keyFrames };
            var animationScaleY = new DoubleAnimationUsingKeyFrames { KeyFrames = keyFrames };

            Storyboard.SetTargetProperty(animationScaleX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(animationScaleY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            Storyboard.SetTarget(animationScaleX, this);
            Storyboard.SetTarget(animationScaleY, this);

            var sb = new Storyboard
            {
                Children = new TimelineCollection
                {
                    animationScaleX,
                    animationScaleY
                }
            };

            sb.Begin();

            await Task.Delay(400);
        }

        private void DialogWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Animate == false)
            {
                return;
            }

            if (e.NewValue is IDialogViewModel vm)
            {
                storyboard.Children.Clear();

                DoubleAnimation shrinkOutAnimationX = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
                DoubleAnimation shrinkOutAnimationY = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200)
                };

                Storyboard.SetTargetProperty(shrinkOutAnimationX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
                Storyboard.SetTargetProperty(shrinkOutAnimationY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
                storyboard.Children.Add(shrinkOutAnimationX);
                storyboard.Children.Add(shrinkOutAnimationY);
                Storyboard.SetTarget(shrinkOutAnimationX, this);
                Storyboard.SetTarget(shrinkOutAnimationY, this);

                vm.CloseDialogAnimation = CloseAnimation;
            }
            else
            {
                throw new InvalidOperationException("DataContext must implement IDialogViewModel");
            }
        }

        private void CloseAnimation() => storyboard.Begin();
    }
}
