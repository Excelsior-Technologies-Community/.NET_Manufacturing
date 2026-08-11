using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialDispatch
    {
        [Key]
        public int DispatchId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public int? ProductionOrderId { get; set; }

        [Required]
        public int DispatchQuantity { get; set; }

        [Required]
        public DateTime DispatchDate { get; set; }

        public string DispatchedBy { get; set; }

        public string Destination { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        // Display Properties
        public string MaterialName { get; set; }

        public string OrderNo { get; set; }
    }
}
