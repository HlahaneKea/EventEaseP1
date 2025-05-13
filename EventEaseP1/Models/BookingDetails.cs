namespace EventEaseP1.Models
{
    public class BookingDetails
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public DateOnly BookingDate { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
    }
}