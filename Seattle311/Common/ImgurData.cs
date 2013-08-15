using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.Common
{
    public class ImgurData
    {
        [JsonProperty(PropertyName = "data")]
        public ImgurImageData Image { get; set; }
        public bool Success { get; set; }
        public int Status { get; set; }
    }
}
