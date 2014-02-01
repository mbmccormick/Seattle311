using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;

namespace Public311.Controls
{
    public partial class AttributeNumberControl : UserControl
    {
        public Public311.API.Models.Attribute AttributeData { get; set; }

        public int Value
        {
            get
            {
                return Convert.ToInt32(this.txtValue.Text);
            }
        }

        public AttributeNumberControl()
        {
            InitializeComponent();
        }
    }
}
