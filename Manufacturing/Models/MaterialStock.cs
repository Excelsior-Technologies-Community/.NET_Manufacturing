using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialStock
    {
        [Key]
        public int StockId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public int CurrentStock { get; set; }

        public int AddedStock { get; set; }

        public int UpdatedStock { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MaterialName { get; set; }
    }
}
