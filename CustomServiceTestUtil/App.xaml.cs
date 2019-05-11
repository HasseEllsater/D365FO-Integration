using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace CustomServiceTestUtil
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string CustomServices = "/api/services/";
        public const string ExternalCorrelationHeader = "x-ms-dyn-externalidentifier";

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            SetThemeSettings();
        }
        public void SetThemeSettings()
        {
            ThemeHelper.ChooseTheme();
            ServerSettings serverSettings = new ServerSettings();
            serverSettings = Settings.GetServerSettings();
            if (serverSettings.OverrideColorSettings == true)
            {
                if (serverSettings.UseWindowsAccent == true)
                {
                    if (!string.IsNullOrEmpty(serverSettings.SelectedAccent.Name))
                    {
                        var theme = ThemeManager.DetectAppStyle(Application.Current);
                        foreach(Accent accent in ThemeManager.Accents)
                        {
                            if(accent.Name == serverSettings.SelectedAccent.Name)
                            {
                                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(serverSettings.SelectedColor.Key))
                    {
                        ThemeManagerHelper.CreateAppStyleBy(serverSettings.SelectedColor.Value, true);
                    }
                }
            }
            Application.Current.MainWindow.Activate();
        }
    }
}
