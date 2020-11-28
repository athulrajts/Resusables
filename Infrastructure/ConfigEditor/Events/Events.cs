using KEI.Infrastructure;
using Prism.Events;
using System;

namespace ConfigEditor.Events
{
    public class DataContainerOpened : PubSubEvent<Tuple<string,IPropertyContainer>> { }
    public class SaveDataContainer : PubSubEvent<IPropertyContainer> { }
    public class ConfigCompareRequest : PubSubEvent<(string, string)> { }
}
