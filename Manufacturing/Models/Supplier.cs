using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Please enter supplier code.")]
        public string SupplierCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter supplier name.")]
        public string SupplierName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? GSTNumber { get; set; }

        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
