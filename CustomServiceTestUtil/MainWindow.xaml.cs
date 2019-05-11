using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using CustomServiceTestUtil.ViewModels;
using MenuItem = CustomServiceTestUtil.ViewModels.MenuItem;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace CustomServiceTestUtil
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public ProgressDialogController dialog;
        private ServicesAPI _currentService;
        private object _callerPage;

        public MainWindow()
        {
            InitializeComponent();
            this.Icon = null;
            Navigation.Navigation.Frame = new FrameExt() { NavigationUIVisibility = NavigationUIVisibility.Hidden };
            Navigation.Navigation.Frame.Navigated += SplitViewFrame_OnNavigated;
            // Navigate to the home page.
            this.Loaded += (sender, args) => Navigation.Navigation.Navigate(new Uri("Views/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SplitViewFrame_OnNavigated(object sender, NavigationEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.Content;
            this.HamburgerMenuControl.SelectedItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetItem(e.Uri);
            this.HamburgerMenuControl.SelectedOptionsItem = e.ExtraData ?? ((ShellViewModel)this.DataContext).GetOptionsItem(e.Uri);
            GoBackButton.Visibility = Navigation.Navigation.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            Navigation.Navigation.GoBack();
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is MenuItem menuItem && menuItem.IsNavigation)
            {
                Navigation.Navigation.Navigate(menuItem.NavigationDestination, menuItem);
            }
        }

        public async Task ShowProgress(string _title, string _message)
        {
            dialog = await this.ShowProgressAsync(_title, _message);
            dialog.SetIndeterminate();
        }
        public async Task CloseProgress()
        {
            await dialog.CloseAsync();
        }
        public void ShowFlyOut(ServicesAPI _service, object _caller)
        {
            if (!(this.Flyouts.Items[0] is Flyout flyout))
            {
                return;
            }

            _currentService = _service;
            _callerPage = _caller;
            ParameterList.ItemsSource = _currentService.Arguments;
            flyout.IsOpen = !flyout.IsOpen;

            ParameterInput.Text = string.Empty;
            ParameterValue.Text = string.Empty;
            AddMethodInput.IsEnabled = false;

            if (ParameterList.SelectedItem != null)
            {
                if (ParameterList.SelectedItem is ServiceMethod Arguments)
                {
                    ParameterInput.Text = Arguments.Parameter;
                    ParameterValue.Text = Arguments.Value;
                    AddMethodInput.IsEnabled = false;

                }
            }

        }

        private void ParameterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParameterInput.Text = string.Empty;
            ParameterValue.Text = string.Empty;

            SaveMethod.IsEnabled = false;
            AddMethodInput.IsEnabled = false;
            if (ParameterList.SelectedItem != null)
            {
                if (ParameterList.SelectedItem is ServiceMethod Arguments)
                {
                    ParameterInput.Text = Arguments.Parameter;
                    ParameterValue.Text = Arguments.Value;
                    SaveMethod.IsEnabled = true;
                }

            }
            else
            {
                AddMethodInput.IsEnabled = true;
            }
        }

        private void AddMethodInput_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ParameterInput.Text))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(ParameterInput.Text))
            {
                return;
            }

            ServiceMethod serviceMethod = new ServiceMethod
            {
                Parameter = ParameterInput.Text,
                Value = ParameterValue.Text
            };
            _currentService.Arguments.Add(serviceMethod);

            if (_callerPage != null)
            {
                if (_callerPage is IArgumentParameters<ServicesAPI> caller)
                {
                    caller.SaveParameters(_currentService);
                    AddMethodInput.IsEnabled = false;

                }
            }

        }

        private void SaveMethod_Click(object sender, RoutedEventArgs e)
        {
            if (_callerPage != null)
            {
                if (_callerPage is IArgumentParameters<ServicesAPI> caller)
                {
                    ServiceMethod serviceMethod = ParameterList.SelectedItem as ServiceMethod;
                    if (serviceMethod != null)
                    {
                        serviceMethod.Parameter = ParameterInput.Text;
                        serviceMethod.Value = ParameterValue.Text;
                        CollectionViewSource.GetDefaultView(ParameterList.ItemsSource).Refresh();
                        caller.SaveParameters(_currentService);
                    }
                }
            }
        }

        private async void RemoveMethod_Click(object sender, RoutedEventArgs e)
        {
            if (ParameterList.SelectedItem != null)
            {
                if (ParameterList.SelectedItem is ServiceMethod serviceMethod)
                {
                    var res = await InfoBox.ShowMessageAsync(Properties.Resources.ConfirmAction, string.Format(Properties.Resources.ConfirmRemovalOfParm, serviceMethod.Parameter, serviceMethod.Value), MessageDialogStyle.AffirmativeAndNegative);
                    if (res == MessageDialogResult.Affirmative)
                    {
                        _currentService.Arguments.Remove(serviceMethod);
                        if (_callerPage != null)
                        {
                            if (_callerPage is IArgumentParameters<ServicesAPI> caller)
                            {
                                caller.SaveParameters(_currentService);
                            }
                        }
                    }


                }
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ParameterList.SelectedIndex;

            //Index of the selected item
            if (currentIndex > 0)
            {
                int upIndex = currentIndex - 1;
                _currentService.Arguments.Move(upIndex, currentIndex);
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ParameterList.SelectedIndex;
            int maxValue = _currentService.Arguments.Count - 1;
            if (currentIndex != maxValue)
            {
                int downIndex = currentIndex + 1;
                _currentService.Arguments.Move(downIndex, currentIndex);

            }
        }

    

        private void ParameterInput_TextChanged(object sender, TextChangedEventArgs e)
        {
           AddMethodInput.IsEnabled = true;
        }
    }
}
