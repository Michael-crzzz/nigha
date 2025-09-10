namespace Dashboard.Models
{
    public class DashboardViewModel
    {
        public int TotalDispensers { get; set; }
        public int TotalLogs { get; set; }
        public int TotalRefills { get; set; }
        public int TotalChecks { get; set; }
        public int TotalReplacements { get; set; }

        public int ActiveDispensers { get; set; }
        public int MaintenanceDispensers { get; set; }
        public int TodaysLogs { get; set; }
        public int ThisWeeksLogs { get; set; }
        public string Status { get; set; } 
        public string Unit { get; set; }

        // ✅ Chart Data
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();

        // For table
        public List<DispenserLog> RecentLogs { get; set; } = new();
        public List<Unit> Units { get; set; }

        public List<Dispenser> Dispensers { get; set; }

    }


}
