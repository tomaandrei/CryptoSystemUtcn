using System.ComponentModel.DataAnnotations;
using System;

namespace CryptoSystemDissertation.Models
{
    public class ImageDetails
    {
        [Key]
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public string Image { get; set; }

        public Parameters Parameters { get; set; }
    }
}