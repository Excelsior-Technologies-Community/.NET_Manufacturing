using System;
using System.Collections.Generic;

namespace Manufacturing.Models
{
    public class ProductionManagerDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int TodaysProductionCount { get; set; }
        public int RunningOrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public int PendingOrdersCount { get; set; }

        public List<ProductionOrder> TodaysOrders { get; set; } = new List<ProductionOrder>();
        public List<ProductionOrder> RunningOrders { get; set; } = new List<ProductionOrder>();
        public List<ProductionOrder> CompletedOrders { get; set; } = new List<ProductionOrder>();
        public List<ProductionOrder> PendingOrders { get; set; } = new List<ProductionOrder>();
        public List<ProductionOrder> RecentOrders { get; set; } = new List<ProductionOrder>();
    }
}
