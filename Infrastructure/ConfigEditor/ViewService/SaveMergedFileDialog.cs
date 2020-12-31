using KEI.Infrastructure;
using KEI.UI.Wpf.ViewService;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;

namespace ConfigEditor.Dialogs
{
    public class SaveMergedFileDialog : SaveFileDialog
    {
        public static readonly StringKey Shape = new StringKey("Shape", "Right");

        protected override void SetDefaultParameters()
        {
            var filters = new FilterCollection
            {
                new Filter("DataContainer Files", "xcfg"),
                new Filter("Recipe Files", "rcp")
            };

            Parameters.PutValue(Filters, filters);
        }

        protected override void PopulateControls()
        {
            var groupBox = new CommonFileDialogGroupBox("Options", "Save To");
            var combobox = new CommonFileDialogComboBox("Shape");
            combobox.Items.Add(new CommonFileDialogComboBoxItem("Left"));
            combobox.Items.Add(new CommonFileDialogComboBoxItem("Right"));
            combobox.SelectedIndex = 0;
            groupBox.Items.Add(combobox);

            Dialog.Controls.Add(groupBox);
        }

        protected override void PopulateResults()
        {
            base.PopulateResults();

            CommonFileDialogGroupBox gbox = Dialog.Controls["Options"] as CommonFileDialogGroupBox;
            CommonFileDialogComboBox cbox = gbox.Items[0] as CommonFileDialogComboBox;

            Results.PutValue(Shape, cbox.Items[cbox.SelectedIndex].Text);
        }
    }
}
