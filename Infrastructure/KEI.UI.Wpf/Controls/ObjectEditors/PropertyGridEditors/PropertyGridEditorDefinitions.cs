using KEI.Infrastructure;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace KEI.UI.Wpf.Controls
{
    public static class PropertyGridEditorDefinitions
    {
        public static void SetDefaultEditors()
        {
            PropertyGridHelper.ExpandableAttribute = new ExpandableObjectAttribute();

            PropertyGridHelper.RegisterEditor<PropertyGridEditors.IntegerUpDownEditor>("int");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.FloatUpDownEditor>("float");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.DoubleUpDownEditor>("double");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.FileNameEditor>("file");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.FolderNameEditor>("folder");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.ColorEditor>("color");
            PropertyGridHelper.RegisterEditor<PropertyGridEditors.ComboBoxEditor>("enum");
        }
    }
}
