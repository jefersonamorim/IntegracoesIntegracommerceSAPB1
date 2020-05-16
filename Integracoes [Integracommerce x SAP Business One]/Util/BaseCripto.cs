using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracommerce.Util
{
    public static class BaseCripto
    {
        //Method using to Encode, you can use internal, public, private...
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return "Basic " + System.Convert.ToBase64String(plainTextBytes);
        }

        //Method using to Decode, you can use internal, public, private...
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return "Basic " + System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
