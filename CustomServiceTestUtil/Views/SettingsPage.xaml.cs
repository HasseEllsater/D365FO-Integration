using CustomServiceTestUtil.Classes;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace CustomServiceTestUtil.Views
{
    public partial class SettingsPage : Page
    {
        ServerSettings serverSettings = new ServerSettings();
        Uri localUri = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute);


        public static readonly DependencyProperty ColorsProperty = DependencyProperty.Register("Colors",
                                  typeof(List<KeyValuePair<string, Color>>),
                                  typeof(SettingsPage),
                                  new PropertyMetadata(default(List<KeyValuePair<string, Color>>)));

        public List<KeyValuePair<string, Color>> Colors
        {
            get { return (List<KeyValuePair<string, Color>>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
        public SettingsPage()
        {
 
            InitializeComponent();
            this.DataContext = this;

            this.Colors = typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)))
                .ToList();
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != localUri)
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            serverSettings.AADTenant = AADTenant.Text;
            serverSettings.AzureAuthEndpoint = AuthorizationEndpoint.Text;
            serverSettings.WebAppId = WebClientAppId.Text;
            serverSettings.WebAADKey = AADKey.Text;
            if (TimeOut.Value == null)
            {
                TimeOut.Value = 5.0;
            }

            serverSettings.TimeOut = TimeOut.Value;

            Settings.SaveServerSettings(serverSettings);
        }
        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigating += NavigationService_Navigating;  
            serverSettings = Settings.GetServerSettings();
            PopulateFields(serverSettings);
        }
        private void PopulateFields(ServerSettings serverSettings)
        {
            AADTenant.Text = serverSettings.AADTenant;
            AuthorizationEndpoint.Text = serverSettings.AzureAuthEndpoint;
            WebClientAppId.Text = serverSettings.WebAppId;
            AADKey.Text = serverSettings.WebAADKey;
            schemaFolder.Text = Settings.SchemaPath;
            responseFolder.Text = Settings.ResponsePath;
            validationFolder.Text = Settings.ValidationPath;

            AccentSelection.IsChecked = serverSettings.UseWindowsAccent;

            if (serverSettings.TimeOut == null)
            {
                serverSettings.TimeOut = 5.0;
            }

            TimeOut.Value = serverSettings.TimeOut;

            if (serverSettings.OverrideColorSettings == true)
            {
                OverrideColorSettings.IsChecked = true;
            }
            else
            {
                OverrideColorSettings.IsChecked = false;
            }

            if (serverSettings.OverrideColorSettings && serverSettings.UseWindowsAccent)
            {
                if (serverSettings.SelectedAccent != null)
                {
                    foreach (Accent accent in AccentSelector.Items)
                    {
                        if (accent.Name == serverSettings.SelectedAccent.Name)
                        {
                            AccentSelector.SelectedItem = accent;
                            break;
                        }
                    }
                }
            }
            if(serverSettings.OverrideColorSettings && serverSettings.UseWindowsAccent == false)
            {
                if (serverSettings.SelectedColor.Key != null)
                {
                    foreach (KeyValuePair<string, Color> color in ColorsSelector.Items)
                    {
                        if (color.Value == serverSettings.SelectedColor.Value)
                        {
                            ColorsSelector.SelectedItem = serverSettings.SelectedColor;
                            break;
                        }
                    }
                }
            }
            ToggleControls();
        }

        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCustomAccent();
        }

        private void SelectCustomAccent()
        {

            if (AccentSelector.SelectedItem is Accent selectedAccent)
            {
                Accent selected = (Accent)AccentSelector.SelectedItem;
                if (serverSettings.SelectedAccent is null)
                {
                    serverSettings.SelectedAccent = new Accent();
                }
                serverSettings.SelectedAccent.Name = selected.Name;
                serverSettings.SelectedAccent.Resources = selected.Resources;
                SaveSettings();

                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, selected, theme.Item1);
            }
 
        }

        private void ColorsSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCustomColor();
        }

        private void SelectCustomColor()
        {
 
            var selectedColor = this.ColorsSelector.SelectedItem as KeyValuePair<string, Color>?;
            if (selectedColor.HasValue)
            {
                serverSettings.SelectedColor = (KeyValuePair<string, Color>)selectedColor;
                SaveSettings();
                ThemeManagerHelper.CreateAppStyleBy(serverSettings.SelectedColor.Value, true);
            }
    
        }


        private void AccentSelection_Click(object sender, RoutedEventArgs e)
        {
            //Off use windows default accent, or use custom color for accent
            if(AccentSelection.IsChecked == true)
            {
                //Use default accent
                serverSettings.UseWindowsAccent = true;

            }
            else
            {
                //Use custom accent
                serverSettings.UseWindowsAccent = false;

            }
            SaveSettings();
            ToggleControls();
        }

        private void OverrideColorSettings_Click(object sender, RoutedEventArgs e)
        {
            ToggleControls();
            if(OverrideColorSettings.IsChecked == true)
            {
                serverSettings.OverrideColorSettings =true;
            }
            else
            {
                serverSettings.OverrideColorSettings = false;
                ThemeHelper.ChooseTheme();
            }
            SaveSettings();
            Application.Current.MainWindow.Activate();
        }

        private void ToggleControls()
        {
            if (OverrideColorSettings.IsChecked == true)
            {
                CustomizationLabel.Visibility = Visibility.Visible;
                AccentSelection.Visibility = Visibility.Visible;
                CustomizationGrid.Visibility = Visibility.Visible;

                if (AccentSelection.IsChecked == false)
                {
                    AccentSelector.Visibility = Visibility.Collapsed;
                    AccentsLabel.Visibility = Visibility.Collapsed;
                    ColorsSelector.Visibility = Visibility.Visible;
                    ColorsLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    AccentSelector.Visibility = Visibility.Visible;
                    AccentsLabel.Visibility = Visibility.Visible;
                    ColorsSelector.Visibility = Visibility.Collapsed;
                    ColorsLabel.Visibility = Visibility.Collapsed;

                }
            }
            else
            {
                CustomizationGrid.Visibility = Visibility.Collapsed;
                CustomizationLabel.Visibility = Visibility.Collapsed;
                AccentSelection.Visibility = Visibility.Collapsed;
                AccentsLabel.Visibility = Visibility.Collapsed;
                AccentSelector.Visibility = Visibility.Collapsed;
                ColorsSelector.Visibility = Visibility.Collapsed;
                ColorsLabel.Visibility = Visibility.Collapsed;
            }
        }

    }
}
