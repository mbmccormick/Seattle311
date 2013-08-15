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

            attach = new ApplicationBarIconButton();
            attach.IconUri = new Uri("/Resources/attach.png", UriKind.RelativeOrAbsolute);
            attach.Text = "attach";

            // build application bar
            ApplicationBar.Buttons.Add(submit);
            ApplicationBar.Buttons.Add(attach);
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