using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoSystemDissertation.Models
{
    public class SendDetails
    {
        public string ImageId { get; set; }
        public int SenderId { get; set; }
        public string RSAKey { get; set; }
    }
}