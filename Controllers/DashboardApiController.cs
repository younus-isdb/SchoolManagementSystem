using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly SchoolDbContext _context;
        public DashboardApiController(SchoolDbContext context) => _context = context;

        [HttpGet("GetDashboardData")]
        public async Task<IActionResult> GetDashboardData()
        {
            var data = new
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalTeachers = await _context.Teachers.CountAsync(),
                TotalAttendanceToday = await _context.Attendances.CountAsync(a => a.Date == DateTime.Today),
                TotalFeeCollectionToday = await _context.FeeCollections
                                                    .Where(f => f.DatePaid == DateTime.Today)
                                                    .SumAsync(f => f.AmountPaid)
            };

            return Ok(data);
        }
    }
}
