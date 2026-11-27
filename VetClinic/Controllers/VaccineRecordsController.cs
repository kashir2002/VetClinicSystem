using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinic.Data;

namespace VetClinic.Controllers
{
    [Authorize(Roles = "Admin,Veterinarian")]
    public class VaccineRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VaccineRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /VaccineRecords/Create?petId=5
        public async Task<IActionResult> Create(int petId)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet == null) return NotFound();

            ViewData["PetName"] = pet.Name;
            return View(new VaccineRecord
            {
                PetId = petId,
                DateGiven = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        // POST: /VaccineRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,VaccineName,DateGiven,NextDoseDate")] VaccineRecord record)
        {
            var pet = await _context.Pets.FindAsync(record.PetId);
            if (pet == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["PetName"] = pet.Name;
                return View(record);
            }

            _context.VaccineRecords.Add(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Pets", new { id = record.PetId });
        }

        // GET: /VaccineRecords/Edit/3
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.VaccineRecords
                .Include(v => v.Pet)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (record == null) return NotFound();

            ViewData["PetName"] = record.Pet!.Name;
            return View(record);
        }

        // POST: /VaccineRecords/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PetId,VaccineName,DateGiven,NextDoseDate")] VaccineRecord input)
        {
            if (id != input.Id) return NotFound();

            var record = await _context.VaccineRecords
                .Include(v => v.Pet)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (record == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["PetName"] = record.Pet!.Name;
                return View(input);
            }

            // Copy the editable fields; the record stays attached to the same pet
            record.VaccineName = input.VaccineName;
            record.DateGiven = input.DateGiven;
            record.NextDoseDate = input.NextDoseDate;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Pets", new { id = record.PetId });
        }

        // GET: /VaccineRecords/Delete/3
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.VaccineRecords
                .Include(v => v.Pet)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: /VaccineRecords/Delete/3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.VaccineRecords.FindAsync(id);
            if (record == null) return NotFound();

            var petId = record.PetId;
            _context.VaccineRecords.Remove(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Pets", new { id = petId });
        }
    }
}