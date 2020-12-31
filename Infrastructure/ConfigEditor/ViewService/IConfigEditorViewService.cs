using KEI.Infrastructure;

namespace ConfigEditor.Dialogs
{
    public interface IConfigEditorViewService : IViewService
    {
        (string left, string right) BrowseCompairFiles();
        IDataContainer ShowSaveMergeFileDialog();
    }
}
