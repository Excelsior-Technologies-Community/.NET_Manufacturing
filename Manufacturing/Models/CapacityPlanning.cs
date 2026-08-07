namespace Manufacturing.Models
{
    public class CapacityPlanning
    {
        public int CapacityPlanId { get; set; }

        public int MachineId { get; set; }

        public int ProductionOrderId { get; set; }

        public decimal AvailableHours { get; set; }

        public decimal RequiredHours { get; set; }

        public string CapacityStatus { get; set; }

        public DateTime PlanningDate { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MachineName { get; set; }

        public string OrderNo { get; set; }
    }
}
