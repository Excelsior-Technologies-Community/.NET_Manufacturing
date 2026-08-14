using System;
using System.Collections.Generic;

namespace Manufacturing.Models
{
    public class ShiftSupervisorDashboardViewModel
    {
        public int TotalShifts { get; set; }
        public int ActiveShiftsCount { get; set; }
        public int OperatorAssignmentsCount { get; set; }
        public int ActiveMachinesCount { get; set; }
        public int PendingMaintenanceCount { get; set; }
        public int ActiveProductionOrdersCount { get; set; }

        public string SupervisorName { get; set; } = string.Empty;

        public List<Shift> Shifts { get; set; } = new List<Shift>();
        public List<OperatorAssignment> RecentOperatorAssignments { get; set; } = new List<OperatorAssignment>();
        public List<ProductionOrder> ActiveProductionOrders { get; set; } = new List<ProductionOrder>();
        public List<MaintenanceRequest> RecentMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
