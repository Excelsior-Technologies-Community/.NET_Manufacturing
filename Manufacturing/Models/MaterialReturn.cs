using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialReturn
    {
        [Key]
        public int ReturnId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int ReturnQuantity { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        public string? ReturnedBy { get; set; }

        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? MaterialName { get; set; }

        public string? OrderNo { get; set; }
    }
}
