﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoSystemDissertation.Models
{
    public class Sender
    {
        public string ReceiverId { get; set; }
        public string Image { get; set; }
        public string RSAKeyXML { get; set; }
    }
}