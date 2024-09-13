namespace LeadTask2.Models
{
    public class Lead
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Course { get; set; }
        public string SourceFrom { get; set; }
        public string Importance { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public bool EmailValidate { get; set; }
        public bool PhoneValidate { get; set; }
        public string Institution { get; set; }
        public string Query { get; set; }
        public string Servicetype { get; set; }
        public bool Resolved { get; set; }
        public string SyncStatus { get; set; }
        public DateTime SyncTime { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
    }
}
