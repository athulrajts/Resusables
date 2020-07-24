using System.Windows.Controls;
using KEI.Infrastructure.Prism;
using Application.UI;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Application.Engineering.Layout;
using System;
using System.Threading.Tasks;
using Prism.Events;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using KEI.UI.Wpf.Hotkey;
using KEI.Infrastructure;

namespace Application.Engineering
{
    /// <summary>
    /// Interaction logic for EngineeringView.xaml
    /// </summary>
    [RegisterWithRegion(RegionNames.ApplicationShell)]
    public partial class EngineeringView : UserControl
    {
        private List<ContentSection> sections = new List<ContentSection>();
        private DockPanelEx layoutPanel;
        public EngineeringView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<SectionRemoved>().Subscribe(async section =>
            {
                if (sections.Find(x => x.Region == section.Region) is ContentSection cs)
                {
                    await cs.ShrinkAndFadeOut();
                }
            });
            eventAggregator.GetEvent<SectionAdded>().Subscribe(async section =>
            {
                if (sections.Find(x => x.Region == section.Region) is ContentSection cs)
                {
                    await cs.GrowAndFadeIn();
                }
            });

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 5),
                DispatcherPriority.Normal, 
                delegate
                {
                    time.Text = DateTime.Now.ToString("HH:mm tt");
                    date.Text = DateTime.Now.ToString("dd-MMM-yy");
                },
                Dispatcher);

        }

        private void ContentSection_Loaded(object sender, RoutedEventArgs e)
        {
            var newcs = sender as ContentSection;
            if (sections.Find(x => x.Region == newcs.Region) is ContentSection cs)
            {
                var index = sections.IndexOf(cs);
                sections.Remove(cs);
                sections.Insert(index, newcs);
            }
            else
            {
                sections.Add(newcs);
            }
        }

        private void DockPanelEx_Loaded(object sender, RoutedEventArgs e)
        {
            layoutPanel = sender as DockPanelEx;
        }
    }
}
