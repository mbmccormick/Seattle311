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

        public MainPage()
        {
            InitializeComponent();

            Services = new ObservableCollection<Service>();
            ServiceRequests = new ObservableCollection<ServiceRequest>();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            App.Seattle311Client.GetServices((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Services.Clear();

                    foreach (Service item in result)
                    {
                        Services.Add(item);
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
                });
            });
        }

        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Service item = ((FrameworkElement)sender).DataContext as Service;

            App.RootFrame.Navigate(new Uri("/NewServiceRequestPage.xaml?id=" + item.service_code + "&name=" + item.service_name, UriKind.Relative));
        }
    }
}