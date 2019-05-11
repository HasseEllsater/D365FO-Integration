using System;
using System.Linq;
using MahApps.Metro.IconPacks;

namespace CustomServiceTestUtil.ViewModels
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menus
            this.Menu.Add(new MenuItem() { Icon = new PackIconMaterialLight() { Kind = PackIconMaterialLightKind.Home}, Text = "Home", NavigationDestination = new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute) });
            this.Menu.Add(new MenuItem() {Icon = new PackIconSimpleIcons() {Kind = PackIconSimpleIconsKind.Json}, Text = "Run test", NavigationDestination = new Uri("Views/RunPage.xaml", UriKind.RelativeOrAbsolute)});
            this.Menu.Add(new MenuItem() {Icon = new PackIconModern() {Kind = PackIconModernKind.Cogs}, Text = "Configure Services", NavigationDestination = new Uri("Views/ConfigureServicesPage.xaml", UriKind.RelativeOrAbsolute)});
            this.Menu.Add(new MenuItem() {Icon = new PackIconModern() {Kind = PackIconModernKind.NetworkServer}, Text = "Configure Endpoints", NavigationDestination = new Uri("Views/ConfigureEndpointsPage.xaml", UriKind.RelativeOrAbsolute)});
            
            this.OptionsMenu.Add(new MenuItem() {Icon = new PackIconModern() {Kind = PackIconModernKind.Settings}, Text = "Configure Settings", NavigationDestination = new Uri("Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute)});
            this.OptionsMenu.Add(new MenuItem() {Icon = new PackIconMaterialLight() {Kind = PackIconMaterialLightKind.Information}, Text = "About", NavigationDestination = new Uri("Views/AboutPage.xaml", UriKind.RelativeOrAbsolute)});
        }

        public object GetItem(object uri)
        {
            return null == uri ? null : this.Menu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }

        public object GetOptionsItem(object uri)
        {
            return null == uri ? null : this.OptionsMenu.FirstOrDefault(m => m.NavigationDestination.Equals(uri));
        }
    }
}