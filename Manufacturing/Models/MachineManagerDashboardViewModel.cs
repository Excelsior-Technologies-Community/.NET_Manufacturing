using System;
using System.Collections.Generic;

namespace Manufacturing.Models
{
    public class MachineManagerDashboardViewModel
    {
        public int TotalMachinesCount { get; set; }
        public int ActiveMachinesCount { get; set; }
        public int MaintenanceMachinesCount { get; set; }
        public int InactiveMachinesCount { get; set; }

        public int TotalAssignmentsCount { get; set; }
        public int TotalAllocationsCount { get; set; }
        public int PendingMaintenanceCount { get; set; }
        public int InProgressMaintenanceCount { get; set; }

        public string ManagerName { get; set; } = string.Empty;

        public List<Machine> Machines { get; set; } = new List<Machine>();
        public List<MachineAssignment> RecentAssignments { get; set; } = new List<MachineAssignment>();
        public List<MaintenanceRequest> RecentMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public List<MachineAllocation> RecentAllocations { get; set; } = new List<MachineAllocation>();
        public List<MachineAvailability> MachineAvailabilities { get; set; } = new List<MachineAvailability>();
    }
}
