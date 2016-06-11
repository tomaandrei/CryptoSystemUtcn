using System.ComponentModel.DataAnnotations;

namespace CryptoSystemDissertation.Models
{
    public class ImageDetails
    {
        [Key]
        public int ImageID { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public string Image { get; set; }

        public Parameters Parameters { get; set; }
    }
}