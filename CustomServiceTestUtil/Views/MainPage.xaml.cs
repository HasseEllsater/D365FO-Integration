using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomServiceTestUtil.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }



        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Endpoints_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(new Uri("Views/ConfigureEndpointsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Servcies_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.Navigate(new Uri("Views/ConfigureServicesPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
