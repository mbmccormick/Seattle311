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

namespace Public311
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Service> Services { get; set; }
        public static ObservableCollection<ServiceRequest> ServiceRequests { get; set; }

        #endregion

        ApplicationBarIconButton refresh;
        ApplicationBarMenuItem updateUserProfile;
        ApplicationBarMenuItem about;

        private GeoCoordinateWatcher locationService = null;

        private bool isServicesLoaded = false;
        private bool isRecentRequestsLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            Services = new ObservableCollection<Service>();
            ServiceRequests = new ObservableCollection<ServiceRequest>();

            locationService = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            locationService.MovementThreshold = 150;
            locationService.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(locationService_PositionChanged);

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            refresh = new ApplicationBarIconButton();
            refresh.IconUri = new Uri("/Resources/refresh.png", UriKind.RelativeOrAbsolute);
            refresh.Text = "refresh";
            refresh.Click += refresh_Click;

            updateUserProfile = new ApplicationBarMenuItem();
            updateUserProfile.Text = "update user profile";
            updateUserProfile.Click += updateUserProfile_Click;

            about = new ApplicationBarMenuItem();
            about.Text = "about";
            about.Click += about_Click;

            // build application bar
            ApplicationBar.Buttons.Add(refresh);
            ApplicationBar.MenuItems.Add(updateUserProfile);
            ApplicationBar.MenuItems.Add(about);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            locationService.Start();

            if (e.IsNavigationInitiator == false ||
                NavigationContext.QueryString.ContainsKey("IsFirstRun") == true)
            {
                LittleWatson.CheckForPreviousException(true);

                if (App.Public311Client.UserData.IsValid() == false)
                    SetUserProfile();

                if (isServicesLoaded == false ||
                    isRecentRequestsLoaded == false)
                {
                    LoadData();
                }
            }
        }

        private void LoadData()
        {
            GlobalLoading.Instance.IsLoading = true;

            App.Public311Client.GetServices((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Services.Clear();

                    foreach (Service item in result)
                    {
                        Services.Add(item);
                    }

                    isServicesLoaded = true;

                    if (isServicesLoaded &&
                        isRecentRequestsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });

            App.Public311Client.GetRecentServiceRequests((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    ServiceRequests.Clear();

                    foreach (ServiceRequest item in result)
                    {
                        ServiceRequests.Add(item);
                    }

                    isRecentRequestsLoaded = true;

                    if (isServicesLoaded &&
                        isRecentRequestsLoaded)
                    {
                        ToggleLoadingText();
                        ToggleEmptyText();

                        GlobalLoading.Instance.IsLoading = false;
                    }
                });
            });
        }

        private void ToggleLoadingText()
        {
            this.txtServicesLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtServiceRequestsLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ToggleEmptyText()
        {
            if (Services.Count == 0)
            {
                this.txtServicesEmpty.Visibility = System.Windows.Visibility.Visible;
                this.lstServices.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.txtServicesEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.lstServices.Visibility = System.Windows.Visibility.Visible;
            }

            if (ServiceRequests.Count == 0)
            {
                this.txtServiceRequestsEmpty.Visibility = System.Windows.Visibility.Visible;
                this.lstServiceRequests.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.txtServiceRequestsEmpty.Visibility = System.Windows.Visibility.Collapsed;
                this.lstServiceRequests.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void SetUserProfile()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/UserProfilePage.xaml", UriKind.Relative));
            });
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            if (GlobalLoading.Instance.IsLoading) return;

            isServicesLoaded = false;
            isRecentRequestsLoaded = false;

            LoadData();
        }

        private void updateUserProfile_Click(object sender, EventArgs e)
        {
            SetUserProfile();
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

            Service item = ((FrameworkElement)sender).DataContext as Service;

            App.RootFrame.Navigate(new Uri("/NewServiceRequestPage.xaml?id=" + item.service_code + "&name=" + item.service_name, UriKind.Relative));
        }

        private void locationService_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (locationService.Status == GeoPositionStatus.Ready &&
                e.Position.Location.IsUnknown == false)
            {
                this.mapLocation.Center = e.Position.Location;
            }
        }
    }
}