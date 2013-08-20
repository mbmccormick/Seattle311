using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seattle311.API.Models
{
    public class Response<T>
    {
        public T ResponseObject { get; set; }
        public List<ErrorMessage> ErrorMessages { get; set; }
    }
}
