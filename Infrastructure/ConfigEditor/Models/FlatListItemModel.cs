using KEI.Infrastructure;

namespace ConfigEditor.Models
{
    public class DictionaryItemModel
    {
        public PropertyObject Value { get; set; }
        public string Key { get; set; }

        public DictionaryItemModel(PropertyObject dataItem, string parentName)
        {
            Key = string.IsNullOrEmpty(parentName) ? dataItem.Name : $"{parentName}.{dataItem.Name}";
            Value = dataItem;
        }
    }
}
