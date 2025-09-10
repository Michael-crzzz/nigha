using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models
{
    public class DispenserLog
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        public string DispenserID { get; set; }

        [Required]
        public string StaffName { get; set; }

        [Required]
        public string ActionTaken { get; set; } // Refill, Check, Replace

        public string? Remarks { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public bool ReviewedByIPCU { get; set; } = false;

        public DateTime? ReviewDate { get; set; }

        // Navigation property (optional, but helpful for Include())
        [ForeignKey(nameof(DispenserID))]
        public virtual Dispenser? Dispenser { get; set; }

        // ✅ Nullable: Only required when ActionTaken == "Replace"
        public string? Volume { get; set; }
    }
}
