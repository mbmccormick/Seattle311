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
        public string LabelName { get; set; }
        public List<Value> Values { get; set; }

        public AttributeSingleValueListControl()
        {
            InitializeComponent();
        }
    }
}
