using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VeterinariansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeterinariansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Veterinarians  (everyone can see the vets)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var vets = await _context.Veterinarians
                .Include(v => v.Specialties)
                .Include(v => v.Services)
                .OrderBy(v => v.FullName)
                .ToListAsync();

            return View(vets);
        }

        // GET: /Veterinarians/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vet = await _context.Veterinarians
                .Include(v => v.Specialties)
                .Include(v => v.Services)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vet == null) return NotFound();
            return View(vet);
        }

        // GET: /Veterinarians/Create
        public async Task<IActionResult> Create()
        {
            var vm = new VeterinarianFormViewModel();
            await FillOptionsAsync(vm);
            return View(vm);
        }

        // POST: /Veterinarians/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VeterinarianFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await FillOptionsAsync(vm);
                return View(vm);
            }

            var vet = new Veterinarian
            {
                FullName = vm.FullName,
                Phone = vm.Phone,
                Specialties = await _context.Specialties
                    .Where(s => vm.SelectedSpecialtyIds.Contains(s.Id))
                    .ToListAsync(),
                Services = await _context.Services
                    .Where(s => vm.SelectedServiceIds.Contains(s.Id))
                    .ToListAsync()
            };

            _context.Veterinarians.Add(vet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Veterinarians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vet = await _context.Veterinarians
                .Include(v => v.Specialties)
                .Include(v => v.Services)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vet == null) return NotFound();

            var vm = new VeterinarianFormViewModel
            {
                Id = vet.Id,
                FullName = vet.FullName,
                Phone = vet.Phone,
                SelectedSpecialtyIds = vet.Specialties.Select(s => s.Id).ToList(),
                SelectedServiceIds = vet.Services.Select(s => s.Id).ToList()
            };

            await FillOptionsAsync(vm);
            return View(vm);
        }

        // POST: /Veterinarians/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VeterinarianFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await FillOptionsAsync(vm);
                return View(vm);
            }

            var vet = await _context.Veterinarians
                .Include(v => v.Specialties)
                .Include(v => v.Services)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vet == null) return NotFound();

            vet.FullName = vm.FullName;
            vet.Phone = vm.Phone;

            vet.Specialties.Clear();
            vet.Specialties.AddRange(await _context.Specialties
                .Where(s => vm.SelectedSpecialtyIds.Contains(s.Id))
                .ToListAsync());

            vet.Services.Clear();
            vet.Services.AddRange(await _context.Services
                .Where(s => vm.SelectedServiceIds.Contains(s.Id))
                .ToListAsync());

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Veterinarians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == id);
            if (vet == null) return NotFound();

            return View(vet);
        }

        // POST: /Veterinarians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vet = await _context.Veterinarians.FindAsync(id);
            if (vet != null)
            {
                _context.Veterinarians.Remove(vet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Loads all specialties and services for the checkboxes
        private async Task FillOptionsAsync(VeterinarianFormViewModel vm)
        {
            vm.AllSpecialties = await _context.Specialties.OrderBy(s => s.Name).ToListAsync();
            vm.AllServices = await _context.Services.OrderBy(s => s.Name).ToListAsync();
        }
    }
}