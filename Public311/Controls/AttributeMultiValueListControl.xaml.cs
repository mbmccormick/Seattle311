using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using Public311.API.Models;

namespace Public311.Controls
{
    public partial class AttributeMultiValueListControl : UserControl
    {
        public Public311.API.Models.Attribute AttributeData { get; set; }

        public List<string> Value
        {
            get
            {
                List<string> returnValue = new List<string>();

                var index = 0;
                foreach (var item in this.lstValue.Items)
                {
                    if (this.lstValue.SelectedItems.Contains(item))
                    {
                        var itemValue = AttributeData.values[index].key;
                        returnValue.Add(itemValue);
                    }

                    index++;
                }

                return returnValue;
            }
        }

        public AttributeMultiValueListControl()
        {
            InitializeComponent();

            this.lstValue.SummaryForSelectedItemsDelegate = (IList items) =>
            {
                if (items == null || items.Count == 0) return string.Empty;

                return String.Join(", ", ((IEnumerable<object>)items).Select(item => (item as Value).name));
            };
        }
    }
}
