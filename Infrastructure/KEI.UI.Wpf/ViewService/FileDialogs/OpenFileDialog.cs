using System;
using System.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using KEI.Infrastructure;

namespace KEI.UI.Wpf.ViewService
{
    public class OpenFileDialog : BaseFileDialog<CommonOpenFileDialog>
    {
        public static BoolKey AddToMostRecentlyUsedList => new BoolKey("AddToMostRecentlyUsedList", false);
        public static BoolKey AllowNonFileSystemItems => new BoolKey("AllowNonFileSystemItems", false);
        public static BoolKey EnsureFileExists => new BoolKey("EnsureFileExists", true);
        public static BoolKey EnsurePathExists => new BoolKey("EnsurePathExists", true);
        public static BoolKey EnsureReadOnly => new BoolKey("EnsureReadOnly", false);
        public static BoolKey EnsureValidNames => new BoolKey("EnsureValidNames", true);
        public static BoolKey ShowPlacesList => new BoolKey("ShowPlacesList", false);
        public static BoolKey Multiselect => new BoolKey("Multiselect", false);
        public static BoolKey IsFolderPicker => new BoolKey("IsFolderPicker", false);
        public static StringKey DefaultDirectory => new StringKey("DefaultDirectory", AppDomain.CurrentDomain.BaseDirectory);
        public static StringKey InitialDirectory => new StringKey("InitialDirectory", AppDomain.CurrentDomain.BaseDirectory);
        public static Key<FilterCollection> Filters => new Key<FilterCollection>("Filters", new FilterCollection());

        protected override void InitializeDialog()
        {
            Dialog.AddToMostRecentlyUsedList = Parameters.GetValue(AddToMostRecentlyUsedList);
            Dialog.AllowNonFileSystemItems = Parameters.GetValue(AllowNonFileSystemItems);
            Dialog.EnsureFileExists = Parameters.GetValue(EnsureFileExists);
            Dialog.EnsurePathExists = Parameters.GetValue(EnsurePathExists);
            Dialog.EnsureReadOnly = Parameters.GetValue(EnsureReadOnly);
            Dialog.EnsureValidNames = Parameters.GetValue(EnsureValidNames);
            Dialog.Multiselect = Parameters.GetValue(Multiselect);
            Dialog.ShowPlacesList = Parameters.GetValue(ShowPlacesList);
            Dialog.IsFolderPicker = Parameters.GetValue(IsFolderPicker);
            Dialog.DefaultDirectory = Parameters.GetValue(DefaultDirectory);
            Dialog.InitialDirectory = Parameters.GetValue(InitialDirectory);

            foreach (var filter in Parameters.GetValue(Filters))
            {
                Dialog.Filters.Add(new CommonFileDialogFilter(filter.Description, filter.Extenstion));
            }
        }

        protected override void PopulateResults()
        {
            Results.PutValue(FileDialogKeys.FileName, Dialog.FileName);
            
            if (Dialog.Multiselect)
            {
                Results.PutValue(FileDialogKeys.FileNames, Dialog.FileNames.ToArray()); 
            }
        }
    }
}
