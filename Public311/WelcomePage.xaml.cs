using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Public311.API.Models;
using System.Collections.ObjectModel;
using Public311.Common;
using System.Device.Location;
using Public311.Models;
using Public311.API;

namespace Public311
{
    public partial class WelcomePage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Endpoint> Endpoints { get; set; }

        #endregion

        ApplicationBarIconButton refresh;
        ApplicationBarMenuItem about;

        private bool isLoaded = false;

        public WelcomePage()
        {
            InitializeComponent();

            Endpoints = new ObservableCollection<Endpoint>();

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            refresh = new ApplicationBarIconButton();
            refresh.IconUri = new Uri("/Resources/refresh.png", UriKind.RelativeOrAbsolute);
            refresh.Text = "refresh";
            refresh.Click += refresh_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += about_Click;

            // build application bar
            ApplicationBar.Buttons.Add(refresh);
            ApplicationBar.MenuItems.Add(about);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                LittleWatson.CheckForPreviousException(true);

                if (isLoaded == false)
                {
                    LoadData();
                }
            }
        }

        private async void LoadData()
        {
            GlobalLoading.Instance.IsLoading = true;

            IEnumerable<Endpoint> results = await App.MobileService.GetTable<Endpoint>().ReadAsync();

            foreach (Endpoint item in results)
            {
                Endpoints.Add(item);
            }

            isLoaded = true;

            if (isLoaded)
            {
                ToggleLoadingText();
                ToggleEmptyText();
            }

            GlobalLoading.Instance.IsLoading = false;
        }

        private void ToggleLoadingText()
        {
            this.txtEndpointsLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtEndpointsLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ToggleEmptyText()
        {
            if (Endpoints.Count == 0)
            {
                this.txtEndpointsEmpty.Visibility = System.Windows.Visibility.Visible;
                this.lstEndpoints.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.txtEndpointsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.lstEndpoints.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            if (GlobalLoading.Instance.IsLoading) return;

            isLoaded = false;

            LoadData();
        }

        private void about_Click(object sender, EventArgs e)
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
            });
        }

        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (GlobalLoading.Instance.IsLoading) return;

            Endpoint item = ((FrameworkElement)sender).DataContext as Endpoint;
            App.CurrentEndpoint = item;

            App.InitializePublic311Client();

            App.RootFrame.Navigate(new Uri("/MainPage.xaml?IsFirstRun=true", UriKind.Relative));
        }
    }
}