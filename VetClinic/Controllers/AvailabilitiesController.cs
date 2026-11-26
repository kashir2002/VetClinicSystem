using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Availabilities
        public async Task<IActionResult> Index()
        {
            var availabilities = await _context.Availabilities
                .Include(a => a.Veterinarian)
                .OrderBy(a => a.Veterinarian!.FullName)
                .ThenBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(availabilities);
        }

        // GET: /Availabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (availability == null) return NotFound();
            return View(availability);
        }

        // GET: /Availabilities/Create
        public IActionResult Create()
        {
            PopulateVeterinarians();
            return View();
        }

        // POST: /Availabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VeterinarianId,DayOfWeek,StartTime,EndTime")] Availability availability)
        {
            if (await OverlapsAsync(availability))
            {
                ModelState.AddModelError("", "This vet already has working hours that overlap with this time.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateVeterinarians(availability.VeterinarianId);
            return View(availability);
        }

        // GET: /Availabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null) return NotFound();

            PopulateVeterinarians(availability.VeterinarianId);
            return View(availability);
        }

        // POST: /Availabilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VeterinarianId,DayOfWeek,StartTime,EndTime")] Availability availability)
        {
            if (id != availability.Id) return NotFound();

            if (await OverlapsAsync(availability))
            {
                ModelState.AddModelError("", "This vet already has working hours that overlap with this time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailabilityExists(availability.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateVeterinarians(availability.VeterinarianId);
            return View(availability);
        }

        // GET: /Availabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (availability == null) return NotFound();
            return View(availability);
        }

        // POST: /Availabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability != null)
            {
                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------- Helpers ----------

        // Two time ranges overlap when A starts before B ends AND B starts before A ends.
        private Task<bool> OverlapsAsync(Availability availability)
        {
            return _context.Availabilities.AnyAsync(a =>
                a.Id != availability.Id &&
                a.VeterinarianId == availability.VeterinarianId &&
                a.DayOfWeek == availability.DayOfWeek &&
                a.StartTime < availability.EndTime &&
                availability.StartTime < a.EndTime);
        }

        // Fills the vet dropdown: sends the Id, shows the FullName.
        private void PopulateVeterinarians(int? selectedId = null)
        {
            ViewData["VeterinarianId"] = new SelectList(
                _context.Veterinarians.OrderBy(v => v.FullName),
                "Id",
                "FullName",
                selectedId);
        }

        private bool AvailabilityExists(int id)
        {
            return _context.Availabilities.Any(e => e.Id == id);
        }
    }
}