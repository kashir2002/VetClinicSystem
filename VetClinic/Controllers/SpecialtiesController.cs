
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Models;
using VetClinic.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class SpecialtiesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SpecialtiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: SPECIALTYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Specialties.ToListAsync());
    }

    // GET: SPECIALTYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var specialty = await _context.Specialties
            .FirstOrDefaultAsync(m => m.Id == id);
        if (specialty == null)
        {
            return NotFound();
        }

        return View(specialty);
    }

    // GET: SPECIALTYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SPECIALTYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] Specialty specialty)
    {
        if (ModelState.IsValid)
        {
            _context.Add(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(specialty);
    }

    // GET: SPECIALTYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty == null)
        {
            return NotFound();
        }
        return View(specialty);
    }

    // POST: SPECIALTYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Veterinarians")] Specialty specialty)
    {
        if (id != specialty.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(specialty);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecialtyExists(specialty.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(specialty);
    }

    // GET: SPECIALTYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var specialty = await _context.Specialties
            .FirstOrDefaultAsync(m => m.Id == id);
        if (specialty == null)
        {
            return NotFound();
        }

        return View(specialty);
    }

    // POST: SPECIALTYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var specialty = await _context.Specialties.FindAsync(id);
        if (specialty != null)
        {
            _context.Specialties.Remove(specialty);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SpecialtyExists(int? id)
    {
        return _context.Specialties.Any(e => e.Id == id);
    }
}
