using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomServiceTestUtil.Classes
{
    class UtilHelper
    {
        public static string OpenFileDialog(string _filter, string _defaultExtension)
        {
            string fileName = string.Empty;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {

                // Set filter for file extension and default file extension 
                DefaultExt = _defaultExtension,
                Filter = _filter
            };


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                fileName = dlg.FileName;
            }
            return fileName;
        }
        public static bool SaveFileDialog(string _filter, string _defaultExtension,string _fileName,string _outputText)
        {
            bool success = false;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = _defaultExtension,
                Filter = _filter,
                FileName = _fileName
            };

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                File.WriteAllText(dlg.FileName, _outputText);
                success = true;
            }
            return success;
        }
        public static string OpenFolderDialog()
        {
            string folderName = string.Empty;

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    folderName = dialog.SelectedPath;
                }
            }
            return folderName;

        }
    }
}
