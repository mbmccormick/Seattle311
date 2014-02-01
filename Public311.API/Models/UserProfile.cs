using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public311.API.Models
{
    public class UserProfile
    {
        public string device_id;
        public string account_id;
        public string first_name;
        public string last_name;
        public string email;
        public string phone;

        public bool IsValid()
        {
            if (String.IsNullOrEmpty(first_name) ||
                String.IsNullOrEmpty(last_name) ||
                String.IsNullOrEmpty(email) ||
                String.IsNullOrEmpty(phone))
            {
                return false;
            }

            return true;
        }
    }
}
