using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Controllers
{
    public class ExamController : Controller
    {
        private readonly SchoolDbContext _db;

        public ExamController(SchoolDbContext db)
        {
            _db = db;
        }

        // =========================
        // INDEX (Exam Master List)
        // =========================
        public async Task<IActionResult> Index()
        {
            var exams = await _db.Exams
                .Include(e => e.Class)
                .ToListAsync();

            return View(exams);
        }

        // =========================
        // DETAILS (Exam + Results)
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var exam = await _db.Exams
                .Include(e => e.Class)
                .Include(e => e.ExamResults)
                    .ThenInclude(r => r.Student)
                .Include(e => e.ExamResults)
                    .ThenInclude(r => r.Subject)
                .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            ViewBag.ClassId = new SelectList(_db.Classes, "ClassId", "Name");
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (exam.EndDate < exam.StartDate)
                ModelState.AddModelError("", "End Date cannot be earlier than Start Date");

            if (ModelState.IsValid)
            {
                _db.Exams.Add(exam);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClassId = new SelectList(_db.Classes, "ClassId", "Name", exam.ClassId);
            return View(exam);
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _db.Exams.FindAsync(id);
            if (exam == null)
                return NotFound();

            ViewBag.ClassId = new SelectList(_db.Classes, "ClassId", "Name", exam.ClassId);
            return View(exam);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam)
        {
            if (id != exam.ExamId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _db.Update(exam);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClassId = new SelectList(_db.Classes, "ClassId", "Name", exam.ClassId);
            return View(exam);
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _db.Exams
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _db.Exams
                .Include(e => e.ExamResults)
                .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam != null)
            {
                _db.ExamResults.RemoveRange(exam.ExamResults); // delete details first
                _db.Exams.Remove(exam);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
