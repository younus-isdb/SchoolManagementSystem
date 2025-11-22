using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly SchoolDbContext _db;
        private readonly IUploadService _uploadService;

        public BookController(SchoolDbContext db, IUploadService uploadService)
        {
            _db = db;
            _uploadService = uploadService;
        }


        // GET: BookController
        public async Task<ActionResult> Index()
        {
            var books = await _db.Books.ToListAsync();
            return View(books);
        }

        // GET: BookController/Details/5
        public async Task< ActionResult> Details(int id)
        {
            if (id<=0)
            {
                return NotFound();
            }
            var books = await _db.Books.FirstOrDefaultAsync(b => b.BookId == id);

            if (books==null)
            {
                return NotFound();
            }
            return View(books);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    book.AvailableCopies = book.TotalCopies;
                    _db.Books.Add(book);
                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            
            catch
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                return View();
            }
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
