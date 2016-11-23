using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Aeronet.Chart.Options
{
    public class FolderBrowserEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));

            if (edSvc != null)
            {

                // open folder brower dialog to specify a folder 

                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                // initial dialog begining folder
                if (value != null && !string.IsNullOrEmpty(value.ToString()) && Directory.Exists(value.ToString()))
                {
                    dialog.SelectedPath = value.ToString();
                }
                else
                {
                    dialog.SelectedPath = Environment.GetFolderPath(dialog.RootFolder);
                }
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {
                    return dialog.SelectedPath;
                }
            }

            return value;
        }
    }
}
