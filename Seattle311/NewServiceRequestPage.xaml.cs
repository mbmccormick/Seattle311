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
        #region List Properties

        public static ServiceDefinition CurrentService { get; set; }

        #endregion

        private GeoCoordinateWatcher locationService = null;

        private bool isLoaded = false;

        ApplicationBarIconButton submit;
        ApplicationBarIconButton attach;

        CameraCaptureTask cameraCaptureTask;

        string imageUrl = null;

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

                        string name;
                        if (NavigationContext.QueryString.TryGetValue("name", out name))
                        {
                            this.txtTitle.Text = this.txtTitle.Text + " - " + name.ToUpper();
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
            if (imageUrl == null)
            {
                if (MessageBox.Show("Are you sure you want to submit this Service Request without attaching a picture?", "Attach Picture", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;
            }

            ServiceRequest request = new ServiceRequest();

            request.service_code = CurrentService.service_code;
            request.lat = locationService.Position.Location.Latitude;
            request.@long = locationService.Position.Location.Longitude;
            request.description = this.txtDescription.Text;
            request.media_url = imageUrl;

            foreach (UserControl control in this.stkFormFields.Children)
            {
                try
                {
                    AttributeDateTimeControl attribute = control as AttributeDateTimeControl;
                    request.attributes.Add(attribute.AttributeData.code, attribute.Value.ToString());
                }
                catch (Exception ex)
                {
                }

                //try
                //{
                //    AttributeMultiValueListControl attribute = control as AttributeMultiValueListControl;
                //    request.attributes.Add(attribute.AttributeData.code, attribute.Value.ToString());
                //}
                //catch (Exception ex)
                //{
                //}

                try
                {
                    AttributeNumberControl attribute = control as AttributeNumberControl;
                    request.attributes.Add(attribute.AttributeData.code, attribute.Value.ToString());
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeSingleValueListControl attribute = control as AttributeSingleValueListControl;
                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeStringControl attribute = control as AttributeStringControl;
                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }

                try
                {
                    AttributeTextControl attribute = control as AttributeTextControl;
                    request.attributes.Add(attribute.AttributeData.code, attribute.Value);
                }
                catch (Exception ex)
                {
                }
            }

            App.Seattle311Client.CreateServiceRequest((result) =>
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Service Request " + result.token + " was created successfully.");

                    NavigationService.GoBack();
                });

            }, request);
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

                this.stkStandardFields.Children.Add(imageControl);

                byte[] buffer = new byte[16 * 1024];

                using (MemoryStream stream = new MemoryStream())
                {
                    int read = 0;
                    while ((read = e.ChosenPhoto.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, read);
                    }

                    this.UploadToImgur(stream.ToArray());
                }
            }
        }

        public void UploadToImgur(byte[] content, Action<bool> onCompletion = null)
        {
            string apiKey = "8fdb6a32174203e";

            string BOUNDARY = Guid.NewGuid().ToString();
            string HEADER = string.Format("--{0}", BOUNDARY);
            string FOOTER = string.Format("--{0}--", BOUNDARY);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.imgur.com/3/image");
            request.Method = "POST";
            request.Headers["Authorization"] = "Client-ID " + apiKey;
            request.ContentType = "multipart/form-data, boundary=" + BOUNDARY;

            StringBuilder builder = new StringBuilder();
            string base64string = Convert.ToBase64String(content);

            builder.AppendLine(HEADER);
            builder.AppendLine("Content-Disposition: form-data; name=\"image\"");
            builder.AppendLine();
            builder.AppendLine(base64string);
            builder.AppendLine(FOOTER);

            byte[] bData = Encoding.UTF8.GetBytes(builder.ToString());

            request.BeginGetRequestStream((result) =>
            {
                using (Stream s = request.EndGetRequestStream(result))
                {
                    s.Write(bData, 0, bData.Length);
                }

                request.BeginGetResponse((respResult) =>
                {
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(respResult);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string jsonContent = reader.ReadToEnd();
                            ImgurData imageData = JsonConvert.DeserializeObject<ImgurData>(jsonContent);

                            if (imageData != null)
                                imageUrl = imageData.Image.Link;

                            Debug.WriteLine(jsonContent);
                        }
                    }
                    catch (WebException ex)
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            Debug.WriteLine(reader.ReadToEnd());
                        }
                    }
                }, null);
            }, null);
        }
    }
}