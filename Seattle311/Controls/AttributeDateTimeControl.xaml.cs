using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Seattle311.Controls
{
    public partial class AttributeDateTimeControl : UserControl
    {
        public Seattle311.API.Models.Attribute AttributeData { get; set; }

        public DateTime Value
        {
            get
            {
                return this.dtpDateValue.Value.Value.Add(this.dtpTimeValue.Value.Value.TimeOfDay);
            }
        }

        public AttributeDateTimeControl()
        {
            InitializeComponent();
        }
    }
}
