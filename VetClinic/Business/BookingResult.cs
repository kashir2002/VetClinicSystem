namespace VetClinic.Business
{
    public class BookingResult
    {
        public bool Success { get; init; }
        public string? Error { get; init; }
        public Appointment? Appointment { get; init; }

        public static BookingResult Ok(Appointment appointment) =>
            new() { Success = true, Appointment = appointment };

        public static BookingResult Fail(string error) =>
            new() { Success = false, Error = error };
    }
}