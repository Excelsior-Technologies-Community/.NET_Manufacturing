using System.Collections.Generic;

namespace Manufacturing.Models
{
    public class RoleStat
    {
        public string RoleName { get; set; }
        public int UserCount { get; set; }
        public int ActiveCount { get; set; }
    }

    public class DashboardViewModel
    {
        // Current User Info
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }

        // Dynamic System Overview Statistics
        public int TotalUsersCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int InactiveUsersCount { get; set; }

        // Role Breakdown
        public List<RoleStat> RoleStats { get; set; } = new List<RoleStat>();

        // System User List for tables
        public List<User> RecentUsers { get; set; } = new List<User>();

        // Role-Specific Dynamic Metrics
        public int Metric1Count { get; set; }
        public string Metric1Label { get; set; }
        public int Metric2Count { get; set; }
        public string Metric2Label { get; set; }
        public int Metric3Count { get; set; }
        public string Metric3Label { get; set; }
        public int Metric4Count { get; set; }
        public string Metric4Label { get; set; }
    }
}
