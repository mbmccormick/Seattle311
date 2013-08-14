using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.API.Models
{
    public class ServiceDefinition
    {
        public string service_code { get; set; }
        public List<Attribute> attributes { get; set; }
    }

    public class Value
    {
        public string key { get; set; }
        public string name { get; set; }
    }

    public class Attribute
    {
        public bool variable { get; set; }
        public string code { get; set; }
        public string datatype { get; set; }
        public bool required { get; set; }
        public int order { get; set; }
        public string description { get; set; }
        public List<Value> values { get; set; }
    }
}
