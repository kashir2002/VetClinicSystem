namespace VetClinic.Business
{
    public interface IAppointmentService
    {
        Task<BookingResult> BookAsync(string ownerId, int petId, int serviceId, int vetId, DateTime start);

        // Returns null on success, or an error message
        Task<string?> ChangeStatusAsync(int appointmentId, AppointmentStatus newStatus, string userId, bool isAdmin);
    }
}