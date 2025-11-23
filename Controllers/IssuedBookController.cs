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
            if (id<=0)
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

    }
}
