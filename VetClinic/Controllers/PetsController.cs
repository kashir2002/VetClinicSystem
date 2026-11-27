using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Controllers
{
    [Authorize] // any logged-in user
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PetsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // The Id of the logged-in user
        private string CurrentUserId => _userManager.GetUserId(User)!;

        // THE security rule: Admin sees all pets, everyone else only their own.
        // Every action below uses this, so the rule lives in one place.
        private IQueryable<Pet> VisiblePets()
        {
            var pets = _context.Pets.Include(p => p.Owner).AsQueryable();

            if (User.IsInRole("Admin"))
            {
                return pets;
            }

            var userId = CurrentUserId;
            return pets.Where(p => p.OwnerId == userId);
        }

        // GET: /Pets
        public async Task<IActionResult> Index()
        {
            var pets = await VisiblePets().OrderBy(p => p.Name).ToListAsync();
            return View(pets);
        }

        // GET: /Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pet = await VisiblePets().FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        // GET: /Pets/Create
        public IActionResult Create()
        {
            return View(new Pet { BirthDate = DateOnly.FromDateTime(DateTime.Today) });
        }

        // POST: /Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Species,Breed,BirthDate,WeightKg")] Pet pet)
        {
            // The owner is ALWAYS the logged-in user, never something from the form
            pet.OwnerId = CurrentUserId;
            ModelState.Remove(nameof(Pet.OwnerId));

            if (!ModelState.IsValid) return View(pet);

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pet = await VisiblePets().FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        // POST: /Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Breed,BirthDate,WeightKg")] Pet input)
        {
            if (id != input.Id) return NotFound();

            // Load the real pet, through the security filter
            var pet = await VisiblePets().FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

            ModelState.Remove(nameof(Pet.OwnerId));
            if (!ModelState.IsValid) return View(input);

            // Copy only the editable fields; the owner never changes
            pet.Name = input.Name;
            pet.Species = input.Species;
            pet.Breed = input.Breed;
            pet.BirthDate = input.BirthDate;
            pet.WeightKg = input.WeightKg;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pet = await VisiblePets().FirstOrDefaultAsync(p => p.Id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

        // POST: /Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await VisiblePets().FirstOrDefaultAsync(p => p.Id == id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}