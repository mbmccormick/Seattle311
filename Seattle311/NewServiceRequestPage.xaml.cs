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
using Seattle311.Controls;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Seattle311
{
    public partial class NewServiceRequestPage : PhoneApplicationPage
    {
        #region Service Properties

        public static ServiceDefinition CurrentService { get; set; }

        #endregion

        ApplicationBarIconButton submit;
        ApplicationBarIconButton attach;

        private GeoCoordinateWatcher locationService = null;

        private bool isLoaded = false;

        string imageUrl = null;

        public NewServiceRequestPage()
        {
            InitializeComponent();

            locationService = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            locationService.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(locationService_PositionChanged);

            this.BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            submit = new ApplicationBarIconButton();
            submit.IconUri = new Uri("/Resources/submit.png", UriKind.RelativeOrAbsolute);
            submit.Text = "submit";
            submit.Click += submit_Click;

            attach = new ApplicationBarIconButton();
            attach.IconUri = new Uri("/Resources/attach.png", UriKind.RelativeOrAbsolute);
            attach.Text = "attach";
            attach.Click += attach_Click;

            // build application bar
            ApplicationBar.Buttons.Add(submit);
            ApplicationBar.Buttons.Add(attach);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            locationService.Start();

            if (isLoaded == false)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                App.Seattle311Client.GetServiceDefinition((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentService = result;

                        string name;
                        if (NavigationContext.QueryString.TryGetValue("name", out name))
                        {
                            this.txtTitle.Text = name.ToUpper();
                        }

                        if (CurrentService.attributes != null)
                        {
                            foreach (Seattle311.API.Models.Attribute item in CurrentService.attributes.OrderBy(z => z.order))
                            {
                                if (item.datatype == "string")
                                {
                                    var control = new AttributeStringControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                                else if (item.datatype == "number")
                                {
                                    var control = new AttributeNumberControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                                else if (item.datatype == "datetime")
                                {
                                    var control = new AttributeDateTimeControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                                else if (item.datatype == "text")
                                {
                                    var control = new AttributeTextControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                                else if (item.datatype == "singlevaluelist")
                                {
                                    var control = new AttributeSingleValueListControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                                else if (item.datatype == "multivaluelist")
                                {
                                    var control = new AttributeMultiValueListControl();
                                    control.AttributeData = item;

                                    this.stkFormFields.Children.Add(control);
                                }
                            }
                        }

                        isLoaded = true;

                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    });
                }, id);
            }
        }

        private void attach_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            PhotoChooserTask task = new PhotoChooserTask();

            task.ShowCamera = true;
            task.Completed += PhotoChooserTask_Completed;

            task.Show();
        }

        private void PhotoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                this.prgLoading.Visibility = System.Windows.Visibility.Visible;

                BitmapImage photo = new BitmapImage();
                photo.SetSource(e.ChosenPhoto);

                Image imgPhoto = new Image();
                imgPhoto.Margin = new Thickness(12, 0, 12, 18);
                imgPhoto.Source = photo;

                this.stkStandardFields.Children.Add(imgPhoto);

                byte[] data = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    WriteableBitmap bitmap = new WriteableBitmap(photo);

                    bitmap.SaveJpeg(stream, photo.PixelWidth, photo.PixelHeight, 0, 100);
                    data = stream.ToArray();
                }

                App.Seattle311Client.UploadImage((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;

                        imageUrl = result.Image.Link;
                    });
                }, data);
            }
        }

        private void submit_Click(object sender, EventArgs e)
        {
            if (this.prgLoading.Visibility == System.Windows.Visibility.Visible) return;

            this.prgLoading.Visibility = System.Windows.Visibility.Visible;

            ServiceRequest request = new ServiceRequest();

            request.service_code = CurrentService.service_code;
            request.description = this.txtDescription.Text;
            request.lat = locationService.Position.Location.Latitude;
            request.@long = locationService.Position.Location.Longitude;
            request.address = this.txtAddress.Text;
            request.media_url = imageUrl;

            #region Parse Attributes

            bool validationSuccess = true;

            foreach (UserControl control in this.stkFormFields.Children)
            {
                try
                {
                    AttributeDateTimeControl attribute = control as AttributeDateTimeControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.ToString().Length == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    request.attributes.Add(attribute.AttributeData.code, attribute.Value.ToString());
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeMultiValueListControl attribute = control as AttributeMultiValueListControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.Count == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    int index = 0;
                    foreach (var item in attribute.Value)
                    {
                        request.attributes.Add(attribute.AttributeData.code + "][" + index, item);
                        index++;
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeNumberControl attribute = control as AttributeNumberControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.ToString().Length == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    request.attributes.Add(attribute.AttributeData.code, attribute.Value.ToString());
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeSingleValueListControl attribute = control as AttributeSingleValueListControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.ToString().Length == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeStringControl attribute = control as AttributeStringControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.ToString().Length == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeTextControl attribute = control as AttributeTextControl;

                    if (attribute.AttributeData.required == true &&
                        attribute.Value.ToString().Length == 0)
                    {
                        MessageBox.Show(attribute.AttributeData.description + " is a required field.", "Validation Error", MessageBoxButton.OK);
                        validationSuccess = false;
                    }

                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }

                if (validationSuccess == false)
                {
                    this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
            }

            #endregion

            bool anonymous = false;

            if (this.chkAnonymous.IsChecked.HasValue &&
                this.chkAnonymous.IsChecked.Value == true)
                anonymous = true;

            App.Seattle311Client.CreateServiceRequest((result1) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    this.prgLoading.Visibility = System.Windows.Visibility.Collapsed;

                    if (result1.ResponseObject != null)
                    {
                        MessageBox.Show("Your service request was submitted successfully!", "Success", MessageBoxButton.OK);
                        NavigationService.GoBack();
                    }
                    else
                    {
                        MessageBox.Show("Error " + result1.ErrorMessages[0].code + ": " + result1.ErrorMessages[0].description, "Failure", MessageBoxButton.OK);
                    }
                });
            }, request, anonymous);
        }

        private void locationService_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (locationService.Status == GeoPositionStatus.Ready &&
                e.Position.Location.IsUnknown == false)
            {
                this.txtLocation.Text = e.Position.Location.Latitude.ToString() + ", " + e.Position.Location.Longitude.ToString();
            }
        }
    }
}