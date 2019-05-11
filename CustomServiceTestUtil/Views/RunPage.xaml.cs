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
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CustomServiceTestUtil.Views
{
   
    public partial class RunPage : Page, IArgumentParameters<ServicesAPI>, IOutputMessages<string>
    {
        private MainWindow mainWindow;
        private ListOfServices ListOfServices = null;
        private string JSONResponse,JSONRequest;
        private string tmpFilePath = Settings.ResponseTmpFile;

        
        private string machine;
        private bool switchMachine = false;

        public RunPage()
        {
            InitializeComponent();
            PopulateCallOptions();
            mainWindow = Application.Current.MainWindow as MainWindow;
        }

        private void PopulateCallOptions()
        {
            Options option = new Options();

            //option.Name = string.Empty;
            //option.SelectedAction = CallAction.NotSelected;
            //cbOptions.Items.Add(option);

            option = new Options
            {
                Name = Properties.Resources.OptionJSONFile,
                SelectedAction = CallAction.File
            };
            OptionsCombo.Items.Add(option);

            option = new Options
            {
                Name = Properties.Resources.OptionParameters,
                SelectedAction = CallAction.Yes
            };
            OptionsCombo.Items.Add(option);

            option = new Options
            {
                Name = Properties.Resources.OptionNoParameters,
                SelectedAction = CallAction.No
            };
            OptionsCombo.Items.Add(option);
        }
        private void PopulateServiceList(ListOfServices _ListOfServices)
        {
            serviceCombo.ItemsSource = _ListOfServices.Services;
            ListOfServices = _ListOfServices;
        }
        private void OpenJSONFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            JSONFile.Text = UtilHelper.OpenFileDialog(Properties.Resources.JSONFileFilter, Properties.Resources.JSONSuffix);
        }


        private async void EditJSON_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tmpFilePath))
            return;

            if (serviceCombo.SelectedItem is ServicesAPI service)
            {
                string currentSchema = string.Empty;

                if (service.ValidationDirection == Direction.Response)
                {
                    if (File.Exists(Path.Combine(Settings.SchemaPath, service.JSONSchema)))
                    {
                        currentSchema = File.ReadAllText(Path.Combine(Settings.SchemaPath, service.JSONSchema));
                    }
                    else
                    {
                        string schemaPath = Path.Combine(Settings.SchemaPath, service.JSONSchema);
                        string message = string.Format(Properties.Resources.SchemaValidationNotPossible, schemaPath);
                        var res = await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, message, MessageDialogStyle.Affirmative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            return;
                        }
                    }
                }
                AX7Endpoints Endpoint = (AX7Endpoints)MachineCombo.SelectedItem;
                if (Endpoint != null)
                {
                    ResponseResult result = new ResponseResult
                    {
                        Schema = currentSchema,
                        ValidationDirection = service.ValidationDirection,
                        CallPath = service.CallPath,
                        Machine = Endpoint.Name
                    };

                    Navigation.Navigation.Navigate(new JSONEditorPage(result));
                }

            }

        }

        private void MachineCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallServiceButton.IsEnabled = false;
            AX7Endpoints Endpoint = (AX7Endpoints)MachineCombo.SelectedItem;
            if (Endpoint != null)
            {
                SelectedEndpoint.Content = string.Format(Properties.Resources.BaseURLString, Endpoint.Machine, Endpoint.EndPointURI);
                if(string.IsNullOrEmpty(machine))
                {
                    machine = SelectedEndpoint.Content.ToString();
                } 
                else if(machine != SelectedEndpoint.Content.ToString())
                {
                    machine = SelectedEndpoint.Content.ToString();
                    switchMachine = true;
                }
                if (serviceCombo.SelectedIndex != -1)
                {
                    ServicesAPI serviceAPI = (ServicesAPI)serviceCombo.Items[serviceCombo.SelectedIndex];
                    if (serviceAPI != null)
                    {
                        if (!String.IsNullOrEmpty(serviceAPI.CallPath))
                        {
                            CallServiceButton.IsEnabled = true;
                        }
                    }

                }
            }
        }

        private void OptionsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (OptionsCombo.SelectedIndex == -1)
            {
                SaveJSONFileLabel.Visibility        = System.Windows.Visibility.Collapsed;
                SaveJSONFileParms.Visibility        = System.Windows.Visibility.Collapsed;
                SetParametersButton.IsEnabled       = false;
                SetParametersButton.Visibility      = System.Windows.Visibility.Hidden;
                return;
            }

            Options option = (Options)OptionsCombo.SelectedItem;
            ToggleParmInput(option.SelectedAction);
        }

        private void ToggleParmInput(CallAction _action)
        {
            SaveJSONFileLabel.Visibility = System.Windows.Visibility.Collapsed;
            SaveJSONFileParms.Visibility = System.Windows.Visibility.Collapsed;
            SetParametersButton.IsEnabled = false;
            SetParametersButton.Visibility = System.Windows.Visibility.Hidden;

            if (_action == CallAction.Yes)
            {
                SetParametersButton.IsEnabled = true;
                SetParametersButton.Visibility = System.Windows.Visibility.Visible;

            }
            else if(_action == CallAction.File)
            {
                SaveJSONFileLabel.Visibility = System.Windows.Visibility.Visible;
                SaveJSONFileParms.Visibility = System.Windows.Visibility.Visible;

            }

            foreach (Options option in OptionsCombo.Items)
            {
                if (option.SelectedAction == _action)
                {
                    OptionsCombo.SelectedItem = option;
                    break;
                }
            }
        }

        private async void CallServiceButton_Click(object sender, RoutedEventArgs e)
        {
            OpenResponseButton.IsEnabled = false;

            if (serviceCombo.SelectedItem != null && OptionsCombo.SelectedItem != null && MachineCombo.SelectedItem != null)
            {
                if (serviceCombo.SelectedItem is ServicesAPI service)
                {
                    ServerSettings serverSettings = Settings.GetServerSettings();
                    serverSettings.Ax7Endpoint = SelectedEndpoint.Content.ToString();
                    Settings.SaveServerSettings(serverSettings);
                    string json = string.Empty;
                    JSONRequest = string.Empty;

                    if (OptionsCombo.SelectedIndex != -1)
                    {
                        Options option = (Options)OptionsCombo.SelectedItem;
                        switch (option.SelectedAction)
                        {
                            case CallAction.File:
                                Output.Text = Properties.Resources.CallingWithFile;
                                json = this.CallWithFile();
                                break;
                            case CallAction.No:
                                Output.Text = Properties.Resources.CallingWithoutParameters;
                                CallService(string.Empty, service.CallPath);
                                break;
                            case CallAction.Yes:
                                json = this.SetParmsAndCall();
                                break;
                            default:
                                return;
                        }
                    }

                    bool validatedOK = true;

                    if (service.ValidationDirection == Direction.Call)
                    {
                        if (!string.IsNullOrEmpty(service.JSONSchema))
                        {
                            if (service.JSONSchema != Properties.Resources.NoSchema)
                            {
                                //Validera mot schema
                                validatedOK = ValidateJSON(json, service.JSONSchema);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(json) && validatedOK)
                    {
                        OpenResponseButton.IsEnabled = false;
                        CallService(json, service.CallPath);
                    }

                }
            }
            else
            {
                await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, Properties.Resources.NotPossibleToCall);
            }


        }

        private bool ValidateJSON(string _json, string _schema, NoYes _showInOutput = NoYes.No)
        {

            bool returnValue = true;
            string errorMessage = string.Empty;

            try
            {
                string currentSchema = File.ReadAllText(string.Format(@"{0}\{1}", Settings.SchemaPath, _schema));

                JSchema schema = JSchema.Parse(currentSchema);
                JToken jsonCall = JToken.Parse(_json);

                returnValue = jsonCall.IsValid(schema, out IList<string> messages);

                if (returnValue == false)
                {
                    foreach (var error in messages)
                    {
                        errorMessage += string.Format("{0}{1}{2}", Environment.NewLine, error, Environment.NewLine);
                    }

                    JSONValidation jsonValidation = new JSONValidation
                    {
                        ValidationResult = errorMessage,
                        Schema = string.Empty,
                        ValidationPerformed = true
                    };

                    AX7Endpoints Endpoint = (AX7Endpoints)MachineCombo.SelectedItem;
                    if (Endpoint != null)
                    {

                        if (serviceCombo.SelectedItem is ServicesAPI service)
                        {
                            ResponseResult result = new ResponseResult
                            {
                                Schema = currentSchema,
                                ValidationDirection = service.ValidationDirection,
                                CallPath = service.CallPath,
                                Machine = Endpoint.Name
               
                            };
                            Navigation.Navigation.Navigate(new ValidateJSONPage(jsonValidation, result));
                        }

                    }
                    Output.Text += string.Format(Properties.Resources.ValidationFailed, Environment.NewLine);
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Validering av schema misslyckades", MessageBoxButtons.OK);
                //returnValue = false;
            }
            return returnValue;
        }
        private string SetParmsAndCall()
        {
            var jsonObject = new Newtonsoft.Json.Linq.JObject();
            string json = string.Empty;
            ServicesAPI service = serviceCombo.SelectedItem as ServicesAPI;

            foreach (ServiceMethod argument in service.Arguments)
            {
                jsonObject.Add(argument.Parameter, argument.Value);
            }

            json = jsonObject.ToString();
            JSONRequest = json;
            Output.Text = string.Format(Properties.Resources.CallingWithParameters,Environment.NewLine,JSONRequest);

            return json;
        }
        private void CallService(string _json, string _service)
        {
            JSONHttpHelper helper = new JSONHttpHelper();
            helper.callService(_json, this,DateTime.Now, _service,switchMachine);
            switchMachine = false;
        }
        private string CallWithFile()
        {
            string json = string.Empty;
            if (Path.GetExtension(JSONFile.Text).ToLower() == ".json")
            {
                json = CreateFromJSON();
                if (!string.IsNullOrEmpty(json))
                {
                    Output.Text = string.Format(Output.Text,Properties.Resources.InputFileSerialized);
                }
            }
            return json;
        }
        private string CreateFromJSON()
        {
            string json = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(JSONFile.Text))
                {
                    json = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //JsonConvert.SerializeObject(json, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            //    PreserveReferencesHandling = PreserveReferencesHandling.Objects
            //});

            return json;
        }
 
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            PopulateServiceList(Settings.GetServiceList());
            PopulateMachinList(Settings.GetEndPointList());
        }

        private void SetParametersButton_Click(object sender, RoutedEventArgs e)
        {
            if (serviceCombo.SelectedItem is ServicesAPI service)
            {
                if (service.UseParameters == CallAction.Yes)
                {
                    mainWindow.ShowFlyOut(service, this);
                }
            }
        }
        private void ServiceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetParametersButton.IsEnabled = false;
            CallServiceButton.IsEnabled = false;
            SchemaLabel.Visibility = Visibility.Collapsed;
            Schema.Visibility = Visibility.Collapsed;
            Schema.Content = string.Empty;

            if (serviceCombo.SelectedItem is ServicesAPI service)
            {
                if (MachineCombo.SelectedIndex != -1)
                {
                    CallServiceButton.IsEnabled = true;
                }
                ToggleParmInput(service.UseParameters);
                if (!string.IsNullOrEmpty(service.JSONSchema))
                {
                    SchemaLabel.Visibility = Visibility.Visible;
                    Schema.Visibility = Visibility.Visible;
                    Schema.Content = string.Format(Properties.Resources.SchemaValidation, service.JSONSchema, EnumHelper.GetDescription(service.ValidationDirection));
                }
            }
        }
        private void PopulateMachinList(ObservableCollection<AX7Endpoints> _endPointList)
        {
            MachineCombo.ItemsSource = _endPointList;
        }

        public void SaveParameters(ServicesAPI serviceapi)
        {
            Settings.SaveServiceList(ListOfServices);
        }
        private void SaveJSONFile(string _json)
        {
            if (!string.IsNullOrWhiteSpace(tmpFilePath))
            {
                if (File.Exists(tmpFilePath))
                {
                    File.Delete(tmpFilePath);
                }
                File.WriteAllText(tmpFilePath, _json);
            }
        }
        public void SetOutput(string output)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => SetOutput(output));
                return;
            }
            Output.Text = string.Format("{0}{1}{2}", Output.Text, Environment.NewLine,output);
        }

        public void SetResponse(string response)
        {
            if (!CheckAccess())
            {
                // On a different thread
                Dispatcher.Invoke(() => SetResponse(response));
                return;
            }
            JSONResponse = response;
            OpenResponseButton.IsEnabled = true;
            Output.Text = string.Format("{0}{1}{2}", Output.Text, Environment.NewLine, Properties.Resources.OpenTheResponse);

            this.SaveJSONFile(JSONResponse);
        }
    }
}
