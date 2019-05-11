using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CustomServiceTestUtil.Views
{
    public partial class ConfigureEndpointsPage : Page
    {
        private MainWindow mainWindow;
        Uri localUri = new Uri("Views/ConfigureEndpointsPage.xaml", UriKind.RelativeOrAbsolute);
        const int NotSelected = 0;

        private ObservableCollection<AX7Endpoints> endPointList;

        public ConfigureEndpointsPage()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow as MainWindow;
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            endPointList = Settings.GetEndPointList();
            MachineDataGrid.ItemsSource = endPointList;
        }
        private void MachineDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MachineDataGrid.SelectedItems.Count != NotSelected)
                {
                    if (MachineDataGrid.SelectedItem is AX7Endpoints item)
                    {
                        Name.Text = item.Name;
                        Machine.Text = item.Machine;
                        EndPoint.Text = item.EndPointURI;
                    }
                    else
                    {
                        Name.Text = string.Empty;
                        Machine.Text = string.Empty;
                        EndPoint.Text = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddEndpoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AX7Endpoints item = new AX7Endpoints
            {
                Name = Name.Text,
                Machine = Machine.Text,
                EndPointURI = EndPoint.Text
            };

            item.URI            = string.Format("https://{0}.{1}", item.Machine, item.EndPointURI);
            bool exist          = false;

            foreach (AX7Endpoints currentItem in endPointList)
            {
                if (currentItem.Name == item.Name && currentItem.Machine == item.Machine && currentItem.URI == item.URI)
                {
                    var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfigurationError, string.Format(Properties.Resources.DuplicateMachine, item.Name, item.URI));
                    exist = true;
                    break;
                }
            }

            if(exist == false)
            {
                endPointList.Add(item);
            }

        }

        private async void RemoveEndpoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MachineDataGrid.SelectedItems.Count != NotSelected)
            {
                if (MachineDataGrid.SelectedItem is AX7Endpoints item)
                {
                    var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, string.Format(Properties.Resources.ConfirmRemoval, item.Machine), MessageDialogStyle.AffirmativeAndNegative);
                    if (res == MessageDialogResult.Affirmative)
                    {
                        endPointList.Remove(item);
                    }

                }
            }
        }
        private void SaveEndpoint_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MachineDataGrid.SelectedItems.Count != NotSelected)
            {
                if (MachineDataGrid.SelectedItem is AX7Endpoints item)
                {

                    item.Name = Name.Text;
                    item.Machine = Machine.Text;
                    item.EndPointURI = EndPoint.Text;
                    item.URI = string.Format("https://{0}.{1}", item.Machine, item.EndPointURI);
                    CollectionViewSource.GetDefaultView(MachineDataGrid.ItemsSource).Refresh();

                }
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Settings.SaveEndPoints(endPointList);
        }
    }
   
}
