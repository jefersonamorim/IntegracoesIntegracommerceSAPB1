using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Entity
{
    public class RetNFResponse
    {
        public object ObjectId { get; set; }
        public string Message { get; set; }
        public Error[] Errors { get; set; }
    }

    public class Error
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

}
