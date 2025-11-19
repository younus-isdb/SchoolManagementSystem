using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class ClassController : Controller
    {
        private readonly SchoolDbContext _context;

        public ClassController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(
                await _context.Departments.ToListAsync(),
                "DepartmentId",
                "DepartmentName"
            );

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class newClass)
        {
            // Fix Navigation Properties to avoid EF Core binding issues
            newClass.Department = null!;
            newClass.Sections = null!;
            newClass.Students = null!;
            newClass.Subjects = null!;
            newClass.ClassSubjects = null!;
            newClass.Exams = null!;
            newClass.Timetables = null!;
            newClass.Assignments = null!;
            newClass.FeeTypes = null!;

            if (!ModelState.IsValid)
            {
                // Re-populate dropdown
                ViewBag.Departments = new SelectList(
                    await _context.Departments.ToListAsync(),
                    "DepartmentId",
                    "DepartmentName",
                    newClass.DepartmentId
                );

                // Debug ModelState errors
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                Console.WriteLine(errors);

                return View(newClass);
            }

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Index (List)
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Department)
                .ToListAsync();
            return View(classes);
        }
    }
}
