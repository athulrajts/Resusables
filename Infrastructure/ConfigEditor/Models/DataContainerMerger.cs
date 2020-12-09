using KEI.Infrastructure;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConfigEditor.Models
{
    public class DataContainerMerger : BindableBase
    {
        public IPropertyContainer Left { get; set; }
        public IPropertyContainer Right { get; set; }

        private ObservableCollection<MergeItem> mergeModel;
        public ObservableCollection<MergeItem> MergedModel
        {
            get { return mergeModel; }
            set { SetProperty(ref mergeModel, value); }
        }

        private ObservableCollection<MergeItem> AllItems { get; set; }

        public DataContainerMerger(IPropertyContainer left, IPropertyContainer right)
        {
            Left = left;
            Right = right;

            MergedModel = new ObservableCollection<MergeItem>();
            AllItems = new ObservableCollection<MergeItem>();

            var leftDic = Left.ToFlatDictionary();
            var rightDic = Right.ToFlatDictionary();

            var allKeys = leftDic.Keys.Union(rightDic.Keys);

            foreach (var key in allKeys)
            {
                var leftData = leftDic.ContainsKey(key) ? leftDic[key] : null;
                var rigthData = rightDic.ContainsKey(key) ? rightDic[key] : null;

                MergedModel.Add(new MergeItem(key, leftData, rigthData));
                AllItems.Add(new MergeItem(key, leftData, rigthData));
            }
        }

        public void ShowDifferentItems(bool showDifferent)
        {
            if (showDifferent)
                MergedModel = new ObservableCollection<MergeItem>(AllItems.Where(x => x.IsDifferent == true).ToList());
            else
                MergedModel = AllItems;
        }

        public void Swap()
        {
            foreach (var item in MergedModel)
            {
                item.Swap();
            }
        }

        public IPropertyContainer Merge(MergeOption shape)
        {
            IPropertyContainer dc = null;
            var leftData = Left.ToFlatDictionary();
            var rightData = Right.ToFlatDictionary();
            var diff = AllItems.Where(x => x.IsDifferent).Select(x => x.Key);

            if(shape == MergeOption.Left)
            {
                dc = (IPropertyContainer)Left.Clone();

                foreach (var key in diff.Intersect(leftData.Keys))
                {
                    var mergeItem = AllItems.FirstOrDefault(x => x.Key == key);
                    object value = mergeItem.Option == MergeOption.Left ? mergeItem.Left?.GetValue() : mergeItem.Right?.GetValue();
                    dc.SetValue(key, rightData[key].GetValue());
                }
            }
            else if(shape == MergeOption.Right)
            {
                dc = (IPropertyContainer)Right.Clone();

                foreach (var key in diff.Intersect(rightData.Keys))
                {
                    var mergeItem = AllItems.FirstOrDefault(x => x.Key == key);
                    object value = mergeItem.Option == MergeOption.Left ? mergeItem.Left?.GetValue() : mergeItem.Right?.GetValue();

                    dc.SetValue(key, value);
                }
            }

            return dc;
        }
    }

    public class MergeItem : BindableBase
    {
        public string Key { get; set; }
        public DataObject Left { get; set; }
        public DataObject Right { get; set; }

        public bool IsDifferent { get; set; }

        private MergeOption option;
        public MergeOption Option
        {
            get { return option; }
            set { SetProperty(ref option, value); }
        }

        public MergeItem(string key, DataObject left, DataObject right)
        {
            Left = left;
            Right = right;
            Key = key;

            IsDifferent = left?.StringValue != right?.StringValue;

            if (IsDifferent && Right is not null)
            {
                Option = MergeOption.Right;
            }
        }

        public void Swap()
        {
            DataObject temp = Left;
            Left = Right;
            Right = temp;

            if (IsDifferent && Right != null)
                Option = MergeOption.Right;

            RaisePropertyChanged(nameof(Left));
            RaisePropertyChanged(nameof(Right));
        }
    }

    public enum MergeOption
    {
        Left,
        Right
    }
}
