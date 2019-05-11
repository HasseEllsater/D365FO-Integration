using CustomServiceTestUtil.Classes;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
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
    partial class ValidateJSONPage : Page
    {
        Uri localUri = new Uri("Views/ValidateJSON.xaml", UriKind.RelativeOrAbsolute);
        JSONValidation jsonValidation;
        ResponseResult responseResult;

        public ValidateJSONPage(JSONValidation _jsonValidation, ResponseResult _result)
        {
            InitializeComponent();
            jsonValidation = _jsonValidation;
            responseResult = _result;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(jsonValidation.ValidationPerformed == false)
            {
                this.ValidateJSON(jsonValidation.JSON, jsonValidation.Schema);
            }
            else
            {
                ValidationResult.Text = jsonValidation.ValidationResult;
            }
        }
        private void ValidateJSON(string _json, string _schema)
        {
            string errorMessage = string.Empty;

            try
            {
     
                JSchema schema = JSchema.Parse(_schema);
                JToken jsonCall = JToken.Parse(_json);

                bool ok = jsonCall.IsValid(schema, out IList<string> messages);

                if (ok == false)
                {
                    foreach (var error in messages)
                    {
                        errorMessage += string.Format("{0}{1}{2}", Environment.NewLine, error, Environment.NewLine);
                    }
                    ValidationResult.Text = errorMessage;
                }
                else
                {
                    ValidationResult.Text = string.Format(Properties.Resources.ValidationOK, Environment.NewLine, _schema);
                }

            }
            catch (Exception ex)
            {
            }

        }

        private void SaveValidationResult_Click(object sender, RoutedEventArgs e)
        {
            FileIOHelper fileIoHelper = new FileIOHelper();
            fileIoHelper.SaveResponse(SaveAction.Validation, ValidationResult.Text, responseResult);
        }
    }
}
