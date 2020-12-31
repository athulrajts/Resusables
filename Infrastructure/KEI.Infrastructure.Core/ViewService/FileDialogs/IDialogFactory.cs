namespace KEI.Infrastructure
{
    public interface IDialogFactory
    {
        public IDataContainer ShowDialog(string dialogName, IDataContainer parameters = null);

        public void RegisterDialog<TDialog>(string name)
            where TDialog : IDialog, new();
    }
}
