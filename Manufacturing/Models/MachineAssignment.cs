using System;
using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MachineAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public string? AssignmentCode { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int MachineId { get; set; }

        public int? EmployeeId { get; set; }

        public int? ShiftId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime AssignmentDate
        {
            get => AssignedDate == default ? DateTime.Today : AssignedDate;
            set => AssignedDate = value;
        }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? Status { get; set; }

        public string? Priority { get; set; }

        public string? AssignedBy { get; set; }

        public string? ApprovedBy { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Display Purpose
        public string? OrderNo { get; set; }

        public string? MachineName { get; set; }

        public string? MachineCode { get; set; }

        public string? EmployeeName { get; set; }

        public string? EmployeeCode { get; set; }

        public string? ProductName { get; set; }

        public string? ShiftName { get; set; }
    }
}
