namespace KEI.Infrastructure
{
    public class SnapShotDiffItem : BindableObject
    {
        private string dataObjectName;
        public string DataObjectName
        {
            get { return dataObjectName; }
            set { SetProperty(ref dataObjectName, value); }
        }

        private string dataObjectType;
        public string DataObjectType
        {
            get { return dataObjectType; }
            set { SetProperty(ref dataObjectType, value); }
        }

        private object left;
        public object Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        private object right;
        public object Right
        {
            get { return right; }
            set { SetProperty(ref right, value); }
        }
    }
}
