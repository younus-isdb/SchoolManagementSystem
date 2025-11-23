// =============================
// FULL STUDENT ADMISSION MODULE
// Includes:
// ✔ Controller Action
// ✔ ViewBag Data Load
// ✔ Validation
// ✔ Create Workflow
// ✔ Photo Upload
// ✔ Auto Roll Generator
// ✔ QR Code ID Card Generation
// =============================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SchoolManagementSystem.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace SchoolManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentController(SchoolDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------------------
        // GET: Student/Create
        // ---------------------------
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "FullName");
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "ClassId", "Name");
            ViewBag.Sections = new SelectList(await _context.Sections.ToListAsync(), "SectionId", "Name");

            return View();
        }

        // ---------------------------
        // POST: Student/Create
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student model, IFormFile Photo)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBags();
                return View(model);
            }

            // Auto Roll No Generator
            model.RollNo = await GenerateRollNo(model.ClassId, model.SectionId);

            // Save Student
            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            // Photo Upload
            if (Photo != null)
                await SaveStudentPhoto(model.StudentId, Photo);

            // Generate QR Code
            await GenerateQrCode(model.StudentId);

            return RedirectToAction("Details", new { id = model.StudentId });
        }

        // ---------------------------
        // Load ViewBags (Reusable)
        // ---------------------------
        private async Task LoadViewBags()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            var classes = await _context.Classes
                .Select(c => new { c.ClassId, c.ClassName })
                .ToListAsync();

            var sections = await _context.Sections
                .Select(s => new { s.SectionId, s.SectionName })
                .ToListAsync();

            ViewBag.Users = new SelectList(users, "UserId", "Name");
            ViewBag.Classes = new SelectList(classes, "ClassId", "ClassName");
            ViewBag.Sections = new SelectList(sections, "SectionId", "SectionName");
        }

        // ---------------------------
        // Auto Roll Generator
        // ---------------------------
        private async Task<string> GenerateRollNo(int classId, int sectionId)
        {
            int count = await _context.Students
                .CountAsync(s => s.ClassId == classId && s.SectionId == sectionId);

            return (count + 1).ToString("D3"); // Example: 001, 002
        }

        // ---------------------------
        // Photo Upload
        // ---------------------------
        private async Task SaveStudentPhoto(int studentId, IFormFile Photo)
        {
            string folder = Path.Combine(_env.WebRootPath, "photos/students");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, studentId + ".jpg");

            using var stream = new FileStream(filePath, FileMode.Create);
            await Photo.CopyToAsync(stream);
        }

        // ---------------------------
        // QR Code Generator
        // ---------------------------
        private async Task GenerateQrCode(int studentId)
        {
            string qrText = $"StudentID:{studentId}";

            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);

            Bitmap qrImage = code.GetGraphic(20);

            string folder = Path.Combine(_env.WebRootPath, "qrcodes/students");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, studentId + ".png");

            qrImage.Save(filePath, ImageFormat.Png);
            await Task.CompletedTask;
        }
    }
}
