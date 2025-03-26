namespace CarRental.Data.Models
{
    public enum RentalStatus
    {
        PendingApproval,
        Approved,
        Declined,
        Reserved,
        Ongoing,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Online
    }
}
