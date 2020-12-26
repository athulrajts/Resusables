using System.Collections.Generic;
using System.ComponentModel;

namespace DataContainer.Tests
{
    public class PropertyChangedListener
    {
        private readonly INotifyPropertyChanged _source;

        public string LastChangedProperty { get; set; }

        public List<string> PropertiesChanged { get; set; } = new List<string>();

        public PropertyChangedListener(INotifyPropertyChanged source)
        {
            _source = source;

            _source.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LastChangedProperty = e.PropertyName;
            PropertiesChanged.Add(e.PropertyName);
        }
    }
}
