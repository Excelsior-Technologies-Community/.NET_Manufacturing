using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class StockVerification
    {
        [Key]
        public int VerificationId { get; set; }

        [Required(ErrorMessage = "Please select a raw material.")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Please enter system stock.")]
        public int SystemStock { get; set; }

        [Required(ErrorMessage = "Please enter physical stock.")]
        public int PhysicalStock { get; set; }

        public int DifferenceStock { get; set; }

        [Required(ErrorMessage = "Please select verification date.")]
        public DateTime VerificationDate { get; set; }

        public string? VerifiedBy { get; set; }

        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? MaterialName { get; set; }
    }
}
