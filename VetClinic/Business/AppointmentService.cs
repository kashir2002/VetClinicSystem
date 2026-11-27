using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Business
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        // Only these statuses block a time slot (a rejected appointment frees it again)
        private static readonly AppointmentStatus[] ActiveStatuses =
        {
            AppointmentStatus.Pending,
            AppointmentStatus.Approved
        };

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResult> BookAsync(string ownerId, int petId, int serviceId, int vetId, DateTime start)
        {
            // 1. Not in the past
            if (start <= DateTime.Now)
                return BookingResult.Fail("The appointment time must be in the future.");

            // 2. The pet must exist AND belong to this owner
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);
            if (pet == null)
                return BookingResult.Fail("Pet not found.");

            // 3. The service must exist
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return BookingResult.Fail("Service not found.");

            // 4. The vet must exist and offer this service
            var vet = await _context.Veterinarians
                .Include(v => v.Services)
                .FirstOrDefaultAsync(v => v.Id == vetId);
            if (vet == null)
                return BookingResult.Fail("Veterinarian not found.");
            if (!vet.Services.Any(s => s.Id == serviceId))
                return BookingResult.Fail($"{vet.FullName} does not offer {service.Name}.");

            // The service duration decides when the appointment ends
            var end = start.AddMinutes(service.DurationMinutes);
            if (end.Date != start.Date)
                return BookingResult.Fail("The appointment must end on the same day.");

            // 5. The whole appointment must fit inside one of the vet's working-hour slots
            var startTime = TimeOnly.FromDateTime(start);
            var endTime = TimeOnly.FromDateTime(end);

            bool withinHours = await _context.Availabilities.AnyAsync(a =>
                a.VeterinarianId == vetId &&
                a.DayOfWeek == start.DayOfWeek &&
                a.StartTime <= startTime &&
                a.EndTime >= endTime);

            if (!withinHours)
                return BookingResult.Fail(
                    $"{vet.FullName} is not available on {start:dddd} from {start:HH:mm} to {end:HH:mm} " +
                    $"({service.Name} takes {service.DurationMinutes} minutes).");

            // 6. No overlap with the vet's other active appointments (same formula as availability!)
            var vetClash = await _context.Appointments
                .Where(a => a.VeterinarianId == vetId && ActiveStatuses.Contains(a.Status))
                .Where(a => a.StartTime < end && start < a.StartTime.AddMinutes(a.DurationMinutes))
                .OrderBy(a => a.StartTime)
                .FirstOrDefaultAsync();

            if (vetClash != null)
                return BookingResult.Fail(
                    $"{vet.FullName} already has an appointment from {vetClash.StartTime:HH:mm} " +
                    $"to {vetClash.EndTime:HH:mm}. Please choose another time.");

            // 7. The pet can't be in two appointments at once
            bool petClash = await _context.Appointments
                .Where(a => a.PetId == petId && ActiveStatuses.Contains(a.Status))
                .AnyAsync(a => a.StartTime < end && start < a.StartTime.AddMinutes(a.DurationMinutes));

            if (petClash)
                return BookingResult.Fail($"{pet.Name} already has another appointment at that time.");

            // All rules passed: create the appointment as Pending
            var appointment = new Appointment
            {
                PetId = petId,
                VeterinarianId = vetId,
                ServiceId = serviceId,
                StartTime = start,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return BookingResult.Ok(appointment);
        }

        public async Task<string?> ChangeStatusAsync(int appointmentId, AppointmentStatus newStatus, string userId, bool isAdmin)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return "Appointment not found.";

            // Admin can manage all; a vet only their own
            if (!isAdmin && appointment.Veterinarian?.UserId != userId)
                return "You can only manage your own appointments.";

            if (newStatus != AppointmentStatus.Approved && newStatus != AppointmentStatus.Rejected)
                return "Invalid status.";

            if (appointment.Status != AppointmentStatus.Pending)
                return "Only pending appointments can be approved or rejected.";

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();
            return null; // success
        }
    }
}