using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Seattle311.Resources;
using Seattle311.API.Models;
using System.Collections.ObjectModel;
using Seattle311.Common;

namespace Seattle311
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region List Properties

        public static ObservableCollection<Service> Services { get; set; }
        public static ObservableCollection<ServiceRequest> ServiceRequests { get; set; }

        #endregion

        ApplicationBarIconButton refresh;
        ApplicationBarMenuItem updateUserProfile;

        private bool isServicesLoaded = false;
        private bool isRecentRequestsLoaded = false;

        public MainPage()
        {
            InitializeComponent();

            Services = new ObservableCollection<Service>();
            ServiceRequests = new ObservableCollection<ServiceRequest>();

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

            // build application bar
            ApplicationBar.Buttons.Add(refresh);
            ApplicationBar.MenuItems.Add(updateUserProfile);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                if (App.Seattle311Client.UserData.IsValid() == false)
                    SetUserProfile();

                if (isServicesLoaded == false ||
                    isRecentRequestsLoaded == false)
                    LoadData();
            }
        }

        private void LoadData()
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            App.Seattle311Client.GetServices((result) =>
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

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });

            App.Seattle311Client.GetRecentServiceRequests((result) =>
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

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    }
                });
            });
        }

        private void ToggleLoadingText()
        {
            this.txtServicesLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.txtServiceRequestsLoading.Visibility = System.Windows.Visibility.Collapsed;

            this.lstServices.Visibility = System.Windows.Visibility.Visible;
            this.lstServiceRequests.Visibility = System.Windows.Visibility.Visible;
        }

        private void ToggleEmptyText()
        {
            if (Services.Count == 0)
                this.txtServicesEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtServicesEmpty.Visibility = System.Windows.Visibility.Collapsed;

            if (ServiceRequests.Count == 0)
                this.txtServiceRequestsEmpty.Visibility = System.Windows.Visibility.Visible;
            else
                this.txtServiceRequestsEmpty.Visibility = System.Windows.Visibility.Collapsed;
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
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            isServicesLoaded = false;
            isRecentRequestsLoaded = false;

            LoadData();
        }

        private void updateUserProfile_Click(object sender, EventArgs e)
        {
            SetUserProfile();
        }

        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            Service item = ((FrameworkElement)sender).DataContext as Service;

            App.RootFrame.Navigate(new Uri("/NewServiceRequestPage.xaml?id=" + item.service_code + "&name=" + item.service_name, UriKind.Relative));
        }
    }
}