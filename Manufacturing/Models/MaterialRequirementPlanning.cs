using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialRequirementPlanning
    {
        [Key]
        public int MaterialPlanId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public int RequiredQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public int ShortageQuantity { get; set; }

        public string Status { get; set; }

        public DateTime PlanningDate { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string OrderNo { get; set; }

        public string MaterialName { get; set; }
    }
}
