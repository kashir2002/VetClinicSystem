using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService)
        {
            _context = context;
            _userManager = userManager;
            _appointmentService = appointmentService;
        }

        private string CurrentUserId => _userManager.GetUserId(User)!;

        // Security rule: Admin sees all, a vet sees their own, a member sees their pets' appointments
        private IQueryable<Appointment> VisibleAppointments()
        {
            var query = _context.Appointments
                .Include(a => a.Pet).ThenInclude(p => p!.Owner)
                .Include(a => a.Veterinarian)
                .Include(a => a.Service)
                .AsQueryable();

            if (User.IsInRole("Admin")) return query;

            var userId = CurrentUserId;
            if (User.IsInRole("Veterinarian"))
                return query.Where(a => a.Veterinarian!.UserId == userId);

            return query.Where(a => a.Pet!.OwnerId == userId);
        }

        // GET: /Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await VisibleAppointments()
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
            return View(appointments);
        }

        // GET: /Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await VisibleAppointments().FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: /Appointments/Create
        public async Task<IActionResult> Create()
        {
            var vm = new AppointmentFormViewModel
            {
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };
            await FillFormAsync(vm);
            return View(vm);
        }

        // POST: /Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await FillFormAsync(vm);
                return View(vm);
            }

            var start = vm.Date!.Value.ToDateTime(vm.Time!.Value);

            // All business rules are in the service layer
            var result = await _appointmentService.BookAsync(
                CurrentUserId, vm.PetId!.Value, vm.ServiceId!.Value, vm.VeterinarianId!.Value, start);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error!);
                await FillFormAsync(vm);
                return View(vm);
            }

            TempData["Message"] = "Your appointment request was sent and is waiting for approval.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Appointments/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Veterinarian")]
        public Task<IActionResult> Approve(int id) =>
            ChangeStatus(id, AppointmentStatus.Approved, "Appointment approved.");

        // POST: /Appointments/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Veterinarian")]
        public Task<IActionResult> Reject(int id) =>
            ChangeStatus(id, AppointmentStatus.Rejected, "Appointment rejected.");

        // ---------- Helpers ----------

        private async Task<IActionResult> ChangeStatus(int id, AppointmentStatus status, string successMessage)
        {
            var error = await _appointmentService.ChangeStatusAsync(
                id, status, CurrentUserId, User.IsInRole("Admin"));

            TempData["Message"] = error ?? successMessage;
            TempData["IsError"] = error != null;
            return RedirectToAction(nameof(Index));
        }

        private async Task FillFormAsync(AppointmentFormViewModel vm)
        {
            var userId = CurrentUserId;

            vm.Pets = await _context.Pets
                .Where(p => p.OwnerId == userId)
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();

            var services = await _context.Services.OrderBy(s => s.Name).ToListAsync();
            vm.Services = services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} ({s.DurationMinutes} min, {s.Price:0.00} TL)"
                })
                .ToList();

            vm.Vets = await _context.Veterinarians
                .Include(v => v.Services)
                .Include(v => v.Availabilities)
                .OrderBy(v => v.FullName)
                .ToListAsync();
        }
    }
}