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

namespace Seattle311.Controls
{
    public partial class AttributeNumberControl : UserControl
    {
        public string LabelName { get; set; }

        public AttributeNumberControl()
        {
            InitializeComponent();
        }
    }
}
