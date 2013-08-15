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
    public partial class AttributeTextControl : UserControl
    {
        public Seattle311.API.Models.Attribute AttributeData { get; set; }

        public AttributeTextControl()
        {
            InitializeComponent();
        }
    }
}
