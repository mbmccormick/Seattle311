using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.Common
{
    public static class ExtendedPropertiesHelper
    {
        public static string Manufacturer
        {
            get
            {
                return DeviceExtendedProperties.GetValue("DeviceManufacturer") as string;
            }
        }

        public static string DeviceUniqueID
        {
            get
            {
                byte[] rawData = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];

                StringBuilder result = new StringBuilder();

                foreach (byte value in rawData)
                {
                    result.Append(Convert.ToInt32(value));
                }

                return result.ToString();
            }
        }

        public static string WindowsLiveAnonymousID
        {
            get
            {
                return UserExtendedProperties.GetValue("ANID2") as string;
            }
        }
    }
}
