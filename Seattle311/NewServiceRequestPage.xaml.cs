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

namespace Seattle311
{
    public partial class NewServiceRequestPage : PhoneApplicationPage
    {
        #region List Properties

        public static ServiceDefinition CurrentService { get; set; }

        #endregion

        private bool isLoaded = false;

        public NewServiceRequestPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
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
                                    control.LabelName = item.description;

                                    this.ContentPanel.Children.Add(control);
                                }
                                else if (item.datatype == "number")
                                {
                                    var control = new AttributeNumberControl();
                                    control.LabelName = item.description;

                                    this.ContentPanel.Children.Add(control);
                                }
                                else if (item.datatype == "datetime")
                                {
                                    var control = new AttributeDateTimeControl();
                                    control.LabelName = item.description;

                                    this.ContentPanel.Children.Add(control);
                                }
                                else if (item.datatype == "text")
                                {
                                    var control = new AttributeTextControl();
                                    control.LabelName = item.description;

                                    this.ContentPanel.Children.Add(control);
                                }
                                else if (item.datatype == "singlevaluelist")
                                {
                                    var control = new AttributeSingleValueListControl();
                                    control.LabelName = item.description;
                                    control.Values = item.values;

                                    this.ContentPanel.Children.Add(control);
                                }
                                else if (item.datatype == "multivaluelist")
                                {
                                    var control = new AttributeMultiValueListControl();
                                    control.LabelName = item.description;

                                    this.ContentPanel.Children.Add(control);
                                }
                            }
                        }

                        isLoaded = true;
                    });
                }, id);
            }
        }
    }
}