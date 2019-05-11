using CustomServiceTestUtil.Classes;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace CustomServiceTestUtil.Views
{
    partial class JSONEditorPage : Page
    {
        Uri localUri = new Uri("Views/JSONEditorPage.xaml", UriKind.RelativeOrAbsolute);
        private string rawJSON,schema;
        private ResponseResult responseResult;
        public JSONEditorPage(ResponseResult _result)
        {
            InitializeComponent();
            responseResult = _result;
            //NavigationService.Navigated += NavigationService_Navigated;
            if (!string.IsNullOrEmpty(responseResult.Schema) && responseResult.ValidationDirection == Direction.Response)
            {
                //Enable validate button
                ValidateResponse.IsEnabled = true;
                schema = responseResult.Schema;
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Settings.ResponseTmpFile))
            {
                rawJSON = File.ReadAllText(Settings.ResponseTmpFile);

                string jsonFormatted = JValue.Parse(rawJSON).ToString(Formatting.Indented);
                JSONEdit.Text = jsonFormatted;
            }

        }

        private void SaveResponse_Click(object sender, RoutedEventArgs e)
        {

            FileIOHelper fileIoHelper = new FileIOHelper();
            fileIoHelper.SaveResponse(SaveAction.Response, JSONEdit.Text, responseResult);
        }

        private void ValidateResponse_Click(object sender, RoutedEventArgs e)
        {
            JSONValidation jsonValidation = new JSONValidation
            {
                JSON = rawJSON,
                Schema = schema,
                ValidationPerformed = false
            };
            Navigation.Navigation.Navigate(new ValidateJSONPage(jsonValidation,responseResult));
        }
    }
}
