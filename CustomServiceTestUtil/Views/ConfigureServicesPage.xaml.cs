using MahApps.Metro.Controls.Dialogs;
using MeasureDataJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CustomServiceTestUtil.Views
{
    public partial class ConfigureServicesPage : Page , IArgumentParameters<ServicesAPI>
    {
        private MainWindow mainWindow;
        Uri localUri = new Uri("Views/ConfigureServicesPage.xaml", UriKind.RelativeOrAbsolute);
        const int NotSelected = 0;
        private ListOfServices currentListOfServices;

        public ConfigureServicesPage()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow as MainWindow;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentListOfServices = Settings.GetServiceList();
            ServiceDataGrid.ItemsSource = currentListOfServices.Services;
            this.LoadSchemas();
            this.LoadOptions();
        }

        private void LoadOptions()
        {
            Options option = new Options();

            option = new Options
            {
                Name = Properties.Resources.OptionJSONFile,
                SelectedAction = CallAction.File
            };
            Parameters.Items.Add(option);

            option = new Options
            {
                Name = Properties.Resources.OptionParameters,
                SelectedAction = CallAction.Yes
            };
            Parameters.Items.Add(option);

            option = new Options
            {
                Name = Properties.Resources.OptionNoParameters,
                SelectedAction = CallAction.No
            };
            Parameters.Items.Add(option);
        }

        private async void LoadSchemas()
        {
            JSONSchemaInfo schemaInfo = new JSONSchemaInfo
            {
                FullPath = string.Empty,
                SchemaFile = Properties.Resources.NoSchema
            };

            JSONSchemas schemas = new JSONSchemas();
            schemas.Schemas.Add(schemaInfo);

            if(string.IsNullOrEmpty(Settings.SchemaPath))
            {
                string message = Properties.Resources.SchemaPathMissing;
                var res = await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, message, MessageDialogStyle.Affirmative);
                return;

            }
            if(!Directory.Exists(Settings.SchemaPath))
            {
                string message = Properties.Resources.SchemaPathInvalid;
                var res = await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, message, MessageDialogStyle.Affirmative);
                return;

            }

            try
            {
                string[] files = Directory.GetFiles(Settings.SchemaPath, "*.json", SearchOption.TopDirectoryOnly);

                foreach (var fullPath in files)
                {
                    int index = fullPath.LastIndexOf(@"\");
                    if (index != -1)
                    {
                        string file = fullPath.Substring(index + 1);
                        JSONSchemaInfo schemaFound = new JSONSchemaInfo
                        {
                            FullPath = fullPath,
                            SchemaFile = file
                        };
                        schemas.Schemas.Add(schemaFound);
                    }
                }

                schema.ItemsSource = schemas.Schemas;

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                var res = await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, message, MessageDialogStyle.Affirmative);
            }

        }

        private void ServiceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ServiceDataGrid.SelectedItems.Count != NotSelected)
                {
                    if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                    {
                        schema.SelectedItem = null;
                        foreach (JSONSchemaInfo item in schema.Items)
                        {
                            if (item.SchemaFile == service.JSONSchema)
                            {
                                schema.SelectedItem = item;
                                break;
                            }
                        }

                        Group.Text = service.Group;
                        Service.Text = service.Service;
                        Method.Text = service.Method;

                        foreach (Options option in Parameters.Items)
                        {
                            if (option.SelectedAction == service.UseParameters)
                            {
                                Parameters.SelectedItem = option;
                                break;
                            }
                        }

                        if (service.UseParameters == CallAction.Yes)
                        {
                            AddParameters.IsEnabled = true;
                        }
                        else
                        {
                            AddParameters.IsEnabled = false;
                        }

                        switch (service.ValidationDirection)
                        {
                            case Direction.Response:
                                get.IsChecked = true;
                                break;

                            case Direction.Call:
                                set.IsChecked = true;
                                break;

                            case Direction.None:
                                notUsed.IsChecked = true;
                                break;

                            default:
                                notUsed.IsChecked = true;
                                break;
                        }
                    }
                    else
                    {
                        Group.Text = string.Empty;
                        Service.Text = string.Empty;
                        Method.Text = string.Empty;
                        AddParameters.IsEnabled = false;
                        notUsed.IsChecked = true;

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddParameters_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceDataGrid.SelectedItems.Count != NotSelected)
            {
                if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                {
                    if (service.UseParameters == CallAction.Yes)
                    {
                        mainWindow.ShowFlyOut(service, this);
                    }
                }
            }
        }

        private async void RemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ServiceDataGrid.SelectedItems.Count != NotSelected)
                {
                    if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                    {
                        string message = string.Format(Properties.Resources.ConfirmRemovalOfService, service.Method);
                        var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, message, MessageDialogStyle.AffirmativeAndNegative);
                        if (res == MessageDialogResult.Affirmative)
                        {
                            currentListOfServices.Services.Remove(service);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveParameters(ServicesAPI serviceAPI)
        {
            Settings.SaveServiceList(currentListOfServices);
        }

        private void SaveMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ServiceDataGrid.SelectedItems.Count != NotSelected)
                {
                    if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                    {
                        service.JSONSchema = default;

                        if (schema.SelectedItem != null)
                        {
                            JSONSchemaInfo schemaInfo = (JSONSchemaInfo)schema.SelectedItem;
                            service.JSONSchema = schemaInfo.SchemaFile;
                        }
 
                        service.Group = Group.Text;
                        service.Service = Service.Text;
                        service.Method = Method.Text;

                        Options option = (Options)Parameters.SelectedItem;
                        service.UseParameters = option.SelectedAction;

                        if (get.IsChecked == true)
                        {
                            service.ValidationDirection = Direction.Response;
                        }

                        if (set.IsChecked == true)
                        {
                            service.ValidationDirection = Direction.Call;
                        }

                        if (notUsed.IsChecked == true)
                        {
                            service.ValidationDirection = Direction.None;
                        }
                        service.CallPath = string.Format(@"{0}/{1}/{2}", service.Group, service.Service, service.Method);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            Settings.SaveServiceList(currentListOfServices);
            CollectionViewSource.GetDefaultView(ServiceDataGrid.ItemsSource).Refresh();
        }

        private async void AddMethod_Click(object sender, RoutedEventArgs e)
        {
            if (Parameters.SelectedItem == null)
            {
                var res = await InfoBox.ShowMessageAsync(Properties.Resources.WarningTitle, Properties.Resources.NoCallParameterSelected);
                return;
            }

            ServicesAPI service = new ServicesAPI();
            if (schema.SelectedItem != null)
            {
                JSONSchemaInfo schemaInfo = (JSONSchemaInfo)schema.SelectedItem;
                service.JSONSchema = schemaInfo.SchemaFile;
            }

            service.Group = Group.Text;
            service.Service = Service.Text;
            service.Method = Method.Text;

            Options option = (Options)Parameters.SelectedItem;
            service.UseParameters = option.SelectedAction;

            if (get.IsChecked == true)
            {
                service.ValidationDirection = Direction.Response;
            }
            else if (set.IsChecked == true)
            {
                service.ValidationDirection = Direction.Call;
            }
            else if (notUsed.IsChecked == true)
            {
                service.ValidationDirection = Direction.None;

            }

            service.CallPath = string.Format(@"{0}/{1}/{2}", service.Group, service.Service, service.Method);
            currentListOfServices.Services.Add(service);
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceDataGrid.SelectedItems.Count != NotSelected)
            {
                if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                {
                    service.ValidationDirection = Direction.Response;
                    CollectionViewSource.GetDefaultView(ServiceDataGrid.ItemsSource).Refresh();
                }
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceDataGrid.SelectedItems.Count != NotSelected)
            {
                if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                {
                    service.ValidationDirection = Direction.Call;
                    CollectionViewSource.GetDefaultView(ServiceDataGrid.ItemsSource).Refresh();
                }
            }
        }

        private void NotUsed_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceDataGrid.SelectedItems.Count != NotSelected)
            {
                if (ServiceDataGrid.SelectedItem is ServicesAPI service)
                {
                    service.ValidationDirection = Direction.None;
                    CollectionViewSource.GetDefaultView(ServiceDataGrid.ItemsSource).Refresh();
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Settings.SaveServiceList(currentListOfServices);
        }

        private void Parameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Parameters.SelectedIndex != -1)
            {
                Options option = (Options)Parameters.SelectedItem;
                if (option.SelectedAction == CallAction.Yes)
                {
                    AddParameters.IsEnabled = true;
                }
                else
                {
                    AddParameters.IsEnabled = false;
                }
                CollectionViewSource.GetDefaultView(ServiceDataGrid.ItemsSource).Refresh();
            }
        }
    }
}