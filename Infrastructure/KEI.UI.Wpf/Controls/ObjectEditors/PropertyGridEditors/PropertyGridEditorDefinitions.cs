﻿using KEI.Infrastructure;

namespace KEI.UI.Wpf.Controls
{
    public static class PropertyGridEditorDefinitions
    {
        public static void SetDefaultEditors()
        {
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.IntegerUpDownEditor>("int");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.FloatUpDownEditor>("float");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.DoubleUpDownEditor>("double");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.FileNameEditor>("file");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.FolderNameEditor>("folder");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.ColorEditor>("color");
            CustomUITypeEditorMapping.RegisterEditor<PropertyGridEditors.PropertyGridEditor>("dc");
        }
    }
}
