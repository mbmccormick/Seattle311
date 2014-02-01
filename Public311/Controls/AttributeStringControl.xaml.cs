using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Public311.Controls
{
    public partial class AttributeStringControl : UserControl
    {
        public Public311.API.Models.Attribute AttributeData { get; set; }

        public string Value
        {
            get
            {
                return this.txtValue.Text;
            }
        }

        public AttributeStringControl()
        {
            InitializeComponent();
        }
    }
}
