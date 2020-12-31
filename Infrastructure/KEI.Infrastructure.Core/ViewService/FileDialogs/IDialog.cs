namespace KEI.Infrastructure
{
    public interface IDialog
    {
        public IDataContainer Results { get; }

        public void ShowDialog();

        public void ShowDialog(IDataContainer parameters);
    }

    public static class FileDialogKeys
    {
        public static StringKey FileName => new StringKey("FileName");
        public static Key<string[]> FileNames => new Key<string[]>("FileNames");
        public static BoolKey DialogResult => new BoolKey("DialogResult");
    }
}
