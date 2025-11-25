using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Services;

namespace SchoolManagementSystem.Controllers
{
    public class IssuedBookController : Controller
    {
        private readonly SchoolDbContext _db;


        public IssuedBookController(SchoolDbContext db)
        {
            _db = db;

        }


        //Get: IssuedBookController
        public async Task<IActionResult> Index()
        {
            var issued = await _db.IssuedBooks
                .Include(a => a.Book)
                .Include(a => a.AppUser)
                .OrderByDescending(a => a.IssueDate).ToListAsync();
            return View(issued);
        }

        // GET: IssuedBookController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issued = await _db.IssuedBooks.Include(a => a.Book).Include(a => a.AppUser).FirstOrDefaultAsync(a => a.Id == id);
            if (issued == null)
            {
                return NotFound();
            }
            return View(issued);
        }

        // // GET: IssuedBookController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
            ViewBag.User = await _db.Users.ToListAsync();
            return View();
        }

        // POST: IssuedBookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IssuedBook issuedBook)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var books = await _db.Books.FindAsync(issuedBook.BookId);
                    if (books == null || books.AvailableCopies <= 0)
                    {
                        ModelState.AddModelError("BookId", "Selected book is not available");
                        ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
                        ViewBag.User = await _db.Users.ToListAsync();
                        return View(issuedBook);
                    }

                    issuedBook.IssueDate = DateTimeOffset.Now;
                    issuedBook.ReturnDate = null;
                    issuedBook.Fine = 0;

                    _db.IssuedBooks.Add(issuedBook);

                    books.AvailableCopies--;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
                ViewBag.User = await _db.Users.ToListAsync();
                return View(issuedBook);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unable to issue book. Please try again.");
                ViewBag.Books = await _db.Books.Where(b => b.AvailableCopies > 0).ToListAsync();
                ViewBag.User = await _db.Users.ToListAsync();
                return View(issuedBook);
            }

        }

        // GET: IssuedBookController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var issuedbook = await _db.IssuedBooks.Include(b => b.Book).Include(b => b.AppUser).FirstOrDefaultAsync(b => b.Id == id);
            if (issuedbook == null)
            {
                return NotFound();
            }

            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Users = await _db.Users.ToListAsync();

            return View(issuedbook);
        }


        // POST: IssuedBookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IssuedBook issuedBook)
        {
            if (id!=issuedBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(issuedBook);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!IssuedBookExists(id))
                    {
                        return NotFound();
                    }
                }
            }
            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Users = await _db.Users.ToListAsync();

            return View(issuedBook);
        }


        // GET: IssuedBookController/Return/5
        public async Task<IActionResult> Return(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issuedBook = await _db.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.AppUser)
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (issuedBook == null)
            {
                return NotFound();
            }

         
            if (issuedBook.ReturnDate == null)
            {
                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (DateTimeOffset.Now > dueDate)
                {
                    var daysLate = (DateTimeOffset.Now - dueDate).Days;
                    issuedBook.Fine = daysLate * 10; // 10 per day
                }
            }

            return View(issuedBook);
        }

        // POST: IssuedBookController/Return/5
        [HttpPost]
        [ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                {
                    return NotFound();
                }

                if (issuedBook.ReturnDate != null)
                {
                    ModelState.AddModelError("", "This book has already been returned.");
                    return View(issuedBook);
                }

               
                issuedBook.ReturnDate = DateTimeOffset.Now;

              
                var dueDate = issuedBook.IssueDate.AddDays(14);
                if (issuedBook.ReturnDate > dueDate)
                {
                    var daysLate = (issuedBook.ReturnDate.Value - dueDate).Days;

                    issuedBook.Fine = daysLate * 10; 
                }

               
                if (issuedBook.Book != null)
                {
                    issuedBook.Book.AvailableCopies++;
                }

                _db.Update(issuedBook);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing the return.");
                return View(await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .Include(ib => ib.AppUser)
                    .FirstOrDefaultAsync(ib => ib.Id == id));
            }
        }

        // GET: IssuedBookController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var issuedBook = await _db.IssuedBooks
                .Include(ib => ib.Book)
                .Include(ib => ib.AppUser)
                .FirstOrDefaultAsync(ib => ib.Id == id);

            if (issuedBook == null)
            {
                return NotFound();
            }

            return View(issuedBook);
        }

        // POST: IssuedBookController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var issuedBook = await _db.IssuedBooks
                    .Include(ib => ib.Book)
                    .FirstOrDefaultAsync(ib => ib.Id == id);

                if (issuedBook == null)
                {
                    return NotFound();
                }

                
                if (issuedBook.ReturnDate == null && issuedBook.Book != null)
                {
                    issuedBook.Book.AvailableCopies++;
                }

                _db.IssuedBooks.Remove(issuedBook);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the record.");

                return View(await _db.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.AppUser).FirstOrDefaultAsync(ib => ib.Id == id));
            }
        }



        private bool IssuedBookExists(int id)
        {
            return _db.IssuedBooks.Any(e => e.Id == id);
        }
    }
}