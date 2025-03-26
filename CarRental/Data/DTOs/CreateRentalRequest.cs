namespace CarRental.Data.DTOs
{
    public class CreateRentalRequest
    {
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}