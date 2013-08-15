using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.Common
{
    public class ImgurImageData
    {
        public string ID { get; set; }
        [JsonProperty(PropertyName = "deletehash")]
        public string DeleteHash { get; set; }
        public string Title { get; set; }
        public Int64 DateTime { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "animated")]
        public bool IsAnimated { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Int64 Size { get; set; }
        public Int64 Views { get; set; }
        [JsonProperty(PropertyName = "account_url")]
        public string AccountUrl { get; set; }
        public string Link { get; set; }
        public string Bandwidth { get; set; }
        public int Ups { get; set; }
        public int Downs { get; set; }
        public int Score { get; set; }
        [JsonProperty(PropertyName = "is_album")]
        public bool IsAlbum { get; set; }
    }
}
