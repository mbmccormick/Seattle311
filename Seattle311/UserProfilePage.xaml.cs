using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Seattle311.API.Models;
using Seattle311.Common;

namespace Seattle311
{
    public partial class UserProfilePage : PhoneApplicationPage
    {
        #region User Profile Properties

        public static UserProfile CurrentUserProfile { get; set; }

        #endregion

        ApplicationBarIconButton save;

        private bool isLoaded = false;

        public UserProfilePage()
        {
            InitializeComponent();

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            save = new ApplicationBarIconButton();
            save.IconUri = new Uri("/Resources/submit.png", UriKind.RelativeOrAbsolute);
            save.Text = "save";
            save.Click += save_Click;

            // build application bar
            ApplicationBar.Buttons.Add(save);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isLoaded == false)
            {
                LoadData();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (App.Seattle311Client.UserData.IsValid() == false)
            {
                if (MessageBox.Show("You have not saved your user profile, and must do so before you can use this application. Are you sure you want to leave?", "User Profile Required", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    while (NavigationService.RemoveBackEntry() != null)
                    {
                        NavigationService.RemoveBackEntry();
                    }

                    base.OnBackKeyPress(e);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        private void LoadData()
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            CurrentUserProfile = App.Seattle311Client.UserData;

            if (CurrentUserProfile.device_id != null)
                this.txtDeviceID.Text = CurrentUserProfile.device_id;

            if (CurrentUserProfile.account_id != null)
                this.txtAccountID.Text = CurrentUserProfile.account_id;

            if (CurrentUserProfile.first_name != null)
                this.txtFirstName.Text = CurrentUserProfile.first_name;

            if (CurrentUserProfile.last_name != null)
                this.txtLastName.Text = CurrentUserProfile.last_name;

            if (CurrentUserProfile.email != null)
                this.txtEmail.Text = CurrentUserProfile.email;

            if (CurrentUserProfile.phone != null)
                this.txtPhone.Text = CurrentUserProfile.phone;

            isLoaded = true;

            this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            CurrentUserProfile.first_name = this.txtFirstName.Text;
            CurrentUserProfile.last_name = this.txtLastName.Text;
            CurrentUserProfile.phone = this.txtPhone.Text;
            CurrentUserProfile.email = this.txtEmail.Text;

            if (CurrentUserProfile.IsValid() == true)
            {
                App.Seattle311Client.UserData = CurrentUserProfile;

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;

                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("You must complete all of the fields before you can save your changes.", "Validation Error", MessageBoxButton.OK);

                this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}