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

namespace Seattle311.Controls
{
    public partial class AttributeSingleValueListControl : UserControl
    {
        public Seattle311.API.Models.Attribute AttributeData { get; set; }

        public AttributeSingleValueListControl()
        {
            InitializeComponent();
        }
    }
}
