namespace Dashboard.Models
{
    public class Dispenser
    {
        public string DispenserID { get; set; }
        public string Location { get; set; }
        public int UnitID { get; set; }
        public string? QRCodeURL { get; set; } 
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }

        public string Status { get; set; } = "Active";

        public string LastRefill { get; set; } = "Not checked yet";
        public Unit Unit { get; set; }
        public string? QRCodeImageBase64 { get; set; }
    }

}
