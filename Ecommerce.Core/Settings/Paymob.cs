using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Settings
{
    public class Paymob
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string HMAC { get; set; }
        public string BaseUrl { get; set; } = "https://accept.paymob.com/v1/";

    }
}
