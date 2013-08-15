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

namespace Seattle311
{
    public partial class NewServiceRequestPage : PhoneApplicationPage
    {
        #region List Properties

        public static ServiceDefinition CurrentService { get; set; }

        #endregion

        private GeoCoordinateWatcher locationService = null;

        private bool isLoaded = false;

        ApplicationBarIconButton submit;
        ApplicationBarIconButton attach;

        CameraCaptureTask cameraCaptureTask;

        public NewServiceRequestPage()
        {
            InitializeComponent();

            locationService = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            locationService.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(locationService_PositionChanged);

            this.BuildApplicationBar();
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
            string id;
            if (NavigationContext.QueryString.TryGetValue("id", out id))
            {
                App.Seattle311Client.GetServiceDefinition((result) =>
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        CurrentService = result;

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
                    });
                }, id);
            }
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

        private void attach_Click(object sender, EventArgs e)
        {
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += cameraCaptureTask_Completed;

            cameraCaptureTask.Show();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void locationService_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (locationService.Status == GeoPositionStatus.Ready &&
                e.Position.Location.IsUnknown == false)
            {
                this.txtLocation.Text = e.Position.Location.Latitude.ToString() + ", " + e.Position.Location.Longitude.ToString();
            }
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                Image imageControl = new Image();
                imageControl.Margin = new Thickness(12, 0, 12, 18);
                imageControl.Source = bmp;

                this.stkFormFields.Children.Add(imageControl);

                this.UploadToImgur(e.ChosenPhoto);
            }
        }

        private void UploadToImgur(Stream chosenPhoto)
        {
            var w = new WebClient();

            var values = new NameValueCollection
{
{"image", Convert.ToBase64String(File.ReadAllBytes(xOut))}
//I only needed to send the image, if you want to send other values, just add them here
};

            w.Headers.Add("Authorization", "Client-ID " + ClientId);
            byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);

            //now process response as you'd like. the link is encapsulated by <link></link> in the response.


            MemoryStream memoryStream = new MemoryStream();
            chosenPhoto.CopyTo(memoryStream);

            byte[] imageData = memoryStream.ToArray();

            string postData = @"key=13b3346e1bd07a47346fd85b523fdd9cba7a1977&image=" + Convert.ToBase64String(imageData);

            WebClient wc = new WebClient();
            Uri u = new Uri("http://api.imgur.com/2/upload.xml");
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_OpenWriteCompleted);
            wc.UploadStringAsync(u, postData);
        }

        private void wc_OpenWriteCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string s = e.Result.ToString();
                MessageBox.Show(s);
            }
        }
    }
}