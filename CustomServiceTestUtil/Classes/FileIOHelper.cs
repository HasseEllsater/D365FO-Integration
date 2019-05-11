using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;

namespace CustomServiceTestUtil.Classes
{
    public class FileIOHelper
    {

        public async void SaveResponse(SaveAction _saveAction, string _output,ResponseResult _responseResult)
        {
            string fileName = default;
            string callPath = _responseResult.CallPath.Replace(@"/", ".");
            string time = DateTime.Now.ToLongTimeString();
            time = time.Replace(":", ".");

            try
            {
                fileName = string.Format("Machine.{0}_{1}_{2}_{3}.JSON", _responseResult.Machine, time, callPath,_saveAction.ToString());


                string outputFolder = default;
                string messageBody= default;

                switch (_saveAction)
                {
                    case SaveAction.Response:
                        outputFolder = Settings.ResponsePath;
                        messageBody = Properties.Resources.ResponseSaved;
                        break;
                    case SaveAction.Validation:
                        outputFolder = Settings.ValidationPath;
                        messageBody = Properties.Resources.ValidationSaved;
                        break;
                    default:
                        string message = string.Format(Properties.Resources.WrongUseOfFunction, "Class FileIOHelper, method: SaveResponse");
                        throw new Exception(message);
                }


                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }


                string saveFilePath = string.Format(@"{0}\{1}", outputFolder, fileName);

                if (!File.Exists(saveFilePath))
                {

                    File.WriteAllText(saveFilePath, _output);
                    string savedFile = string.Format(messageBody, outputFolder, Environment.NewLine, fileName);
                    await InfoBox.ShowMessageAsync(Properties.Resources.FileSaved, savedFile, MessageDialogStyle.Affirmative);
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, message, MessageDialogStyle.Affirmative);
            }
        }
    }
}
