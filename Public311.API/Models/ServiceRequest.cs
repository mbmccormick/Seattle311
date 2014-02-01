using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public311.API.Models
{
    public class ServiceRequest
    {
        public string service_request_id { get; set; }
        public string status { get; set; }
        public string status_notes { get; set; }
        public string service_name { get; set; }
        public string service_code { get; set; }
        public string description { get; set; }
        public string agency_responsible { get; set; }
        public string service_notice { get; set; }
        public string requested_datetime { get; set; }
        public string updated_datetime { get; set; }
        public string expected_datetime { get; set; }
        public string address { get; set; }
        public int address_id { get; set; }
        public int zipcode { get; set; }
        public double? lat { get; set; }
        public double? @long { get; set; }
        public string media_url { get; set; }
        public Dictionary<string, string> attributes { get; set; }

        public ServiceRequest()
        {
            attributes = new Dictionary<string, string>();
        }
    }
}
