using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.API.Models
{
    public class ServiceRequest
    {
        public int service_request_id { get; set; }
        public string status { get; set; }
        public string status_notes { get; set; }
        public string service_name { get; set; }
        public int service_code { get; set; }
        public object description { get; set; }
        public object agency_responsible { get; set; }
        public object service_notice { get; set; }
        public string requested_datetime { get; set; }
        public string updated_datetime { get; set; }
        public string expected_datetime { get; set; }
        public string address { get; set; }
        public int address_id { get; set; }
        public int zipcode { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public string media_url { get; set; }
    }
}
