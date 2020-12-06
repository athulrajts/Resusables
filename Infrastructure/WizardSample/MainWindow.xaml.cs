using KEI.Infrastructure;
using KEI.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WizardSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var test = new Test
            {
                MyProperty = 32,
                MyProperty2 = HorizontalAlignment.Stretch,
                MyProperty3 = new Test { MyProperty = 12, MyProperty2 = HorizontalAlignment.Center },
                MyProperty4 = new List<Test>
                {
                    new Test { MyProperty = 12, MyProperty2 = HorizontalAlignment.Center },
                    new Test { MyProperty = 12, MyProperty2 = HorizontalAlignment.Center }
                }
            };

            var pc = PropertyContainerBuilder.CreateObject("complex", test);

            var dc = DataContainerBuilder.CreateObject("complex", test);


            //var dc = new DataContainer();

            //dc.Add(new IntDataObject("int", 12));
            //dc.Add(new BoolDataObject("bool", true));
            //dc.Add(new EnumDataObject("enum", AlignmentX.Left));

            //dc.Add(new SelectableDataObject("list", 22, new List<int> { 22, 33, 44 }));

            //var dcInner = new DataContainer();
            //dcInner.Add(new IntDataObject("int", 12));
            //dcInner.Add(new BoolDataObject("bool", true));

            //dc.Add(new ContainerDataObject("dc", dcInner));

            var s = XmlHelper.SerializeToString(pc);
            var s2 = XmlHelper.SerializeToString(dc);

            var pcout = XmlHelper.DeserializeFromString<PropertyContainer>(s);

            var dcout = XmlHelper.DeserializeFromString<DataContainer>(s2);
        }
    }

    public class Test
    {
        [Browsable(false)]
        [Description("teststse")]
        public int MyProperty { get; set; }

        [ReadOnly(true)]
        public HorizontalAlignment MyProperty2 { get; set; }
        
        [ReadOnly(true)]
        public Test MyProperty3 { get; set; }
        public List<Test> MyProperty4 { get; set; }
    }
}
