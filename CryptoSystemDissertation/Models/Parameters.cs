using System.ComponentModel.DataAnnotations;

namespace CryptoSystemDissertation.Models
{
    public class Parameters
    {
        [Key]
        public int ParamtersId { get; set; }
        public double Lambda { get; set; }
        public double X { get; set; }
    }
}