using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Controllers
{
    public class ExamResultController : Controller
    {
        private readonly SchoolDbContext _db;

        public ExamResultController(SchoolDbContext db)
        {
            _db = db;
        }

        // =========================
        // INDEX (Results by Exam)
        // =========================
        public async Task<IActionResult> Index(int examId)
        {
            var results = await _db.ExamResults
                .Where(r => r.ExamId == examId)
                .Include(r => r.Student)
                .Include(r => r.Subject)
                .Include(r => r.Exam)
                .ToListAsync();

            ViewBag.ExamId = examId;
            return View(results);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create(int examId)
        {
            ViewBag.ExamId = examId;
            ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name");
            ViewBag.SubjectId = new SelectList(_db.Subjects, "SubjectId", "Name");
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamResult result)
        {
            if (ModelState.IsValid)
            {
                _db.ExamResults.Add(result);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { examId = result.ExamId });
            }

            ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name", result.StudentId);
            ViewBag.SubjectId = new SelectList(_db.Subjects, "SubjectId", "Name", result.SubjectId);
            return View(result);
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _db.ExamResults.FindAsync(id);
            if (result == null)
                return NotFound();

            ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name", result.StudentId);
            ViewBag.SubjectId = new SelectList(_db.Subjects, "SubjectId", "Name", result.SubjectId);
            return View(result);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamResult result)
        {
            if (id != result.ResultId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _db.Update(result);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { examId = result.ExamId });
            }

            ViewBag.StudentId = new SelectList(_db.Students, "StudentId", "Name", result.StudentId);
            ViewBag.SubjectId = new SelectList(_db.Subjects, "SubjectId", "Name", result.SubjectId);
            return View(result);
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _db.ExamResults
                .Include(r => r.Student)
                .Include(r => r.Subject)
                .Include(r => r.Exam)
                .FirstOrDefaultAsync(r => r.ResultId == id);

            if (result == null)
                return NotFound();

            return View(result);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _db.ExamResults.FindAsync(id);

            if (result != null)
            {
                int examId = result.ExamId;
                _db.ExamResults.Remove(result);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { examId });
            }

            return RedirectToAction("Index", "Exam");
        }
    }
}
