using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly SchoolDbContext _context;
        public DashboardController(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var dashboardData = new DashboardViewModel
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalTeachers = await _context.Teachers.CountAsync(),
                TotalClasses = await _context.Classes.CountAsync(),
                TotalSubjects = await _context.Subjects.CountAsync(),
                TotalExams = await _context.Exams.CountAsync(),
                TodayAttendance = await _context.Attendances.CountAsync(a => a.Date == today),
                TotalFeeCollections = await _context.FeeCollections.CountAsync(),
                TotalHostelResidents = await _context.HostelResidents.CountAsync(),
                TotalTransportAssignments = await _context.TransportAssignments.CountAsync(),
                TotalIssuedBooks = await _context.IssuedBooks.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(e => !e.IsDeleted && e.StartDateTime >= today)
            };

            dashboardData.PresentCount = dashboardData.TodayAttendance;
            dashboardData.AbsentCount = dashboardData.TotalStudents - dashboardData.TodayAttendance;

            // Attendance Trend (Last 7 Days)
            dashboardData.AttendanceTrend = await _context.Attendances
                .Where(a => a.Date >= today.AddDays(-6))
                .GroupBy(a => a.Date)
                .Select(g => new ChartData { Label = g.Key.ToString("dd MMM"), Value = g.Count() })
                .ToListAsync();

            // Monthly Fee Collection Trend (Last 6 Months)
            dashboardData.FeeCollectionTrend = await _context.FeeCollections
                .Where(f => f.DatePaid >= today.AddMonths(-5))
                .GroupBy(f => new { f.DatePaid.Year, f.DatePaid.Month })
                .Select(g => new ChartData
                {
                    Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Value = g.Count()
                })
                .ToListAsync();

            // Hostel Occupancy Pie
            dashboardData.HostelOccupancy = await _context.HostelResidents
                .Include(hr => hr.Hostel)
                .GroupBy(hr => hr.Hostel.Name)
                .Select(g => new ChartData { Label = g.Key, Value = g.Count() })
                .ToListAsync();

            // Transport Occupancy Pie
            dashboardData.TransportOccupancy = await _context.TransportAssignments
                .Include(t => t.TransportRoute)
                .GroupBy(t => t.TransportRoute.RouteName)
                .Select(g => new ChartData { Label = g.Key, Value = g.Count() })
                .ToListAsync();



            dashboardData.UpcomingEvents = await _context.Events
    .Where(e => !e.IsDeleted && e.StartDateTime >= DateTime.Today)
    .OrderBy(e => e.StartDateTime)
    .Select(e => new EventSummary
    {
        EventId = e.EventId,
        Title = e.Title,
        StartDateTime = e.StartDateTime
    })
    .Take(5)
    .ToListAsync();

            // Notifications (e.g., from Notices or Messages)
            dashboardData.Notifications = await _context.Notices
                .OrderByDescending(n => n.NoticeId)
                .Take(5)
                .Select(n => new NotificationSummary
                {
                    Id = n.NoticeId,
                    Message = n.Title,
                    CreatedAt = DateTime.Now // Or n.CreatedAt if field exists
                })
                .ToListAsync();

            // Quick Links (Static or Role-Based)
            dashboardData.QuickLinks = new List<QuickLink>
{
    new QuickLink { Title="Add Student", Url="/Students/Create", IconClass="bi bi-person-plus-fill" },
    new QuickLink { Title="Add Teacher", Url="/Teacher/Create", IconClass="bi bi-person-badge-fill" },
    new QuickLink { Title="Mark Attendance", Url="/Attendance/Mark", IconClass="bi bi-check2-square" },
    new QuickLink { Title="Fee Collection", Url="/FeeCollection", IconClass="bi bi-currency-dollar" },
    new QuickLink { Title="Issue Book", Url="/IssuedBook/Create", IconClass="bi bi-book-half" }
};





            return View(dashboardData);
        }
    }

    public class DashboardViewModel
    {
        // Counts
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalExams { get; set; }
        public int TodayAttendance { get; set; }
        public int TotalFeeCollections { get; set; }
        public int TotalHostelResidents { get; set; }
        public int TotalTransportAssignments { get; set; }
        public int TotalIssuedBooks { get; set; }
        public int TotalEvents { get; set; }

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }

        // Advanced Charts
        public List<ChartData> AttendanceTrend { get; set; } = new();
        public List<ChartData> FeeCollectionTrend { get; set; } = new();
        public List<ChartData> HostelOccupancy { get; set; } = new();
        public List<ChartData> TransportOccupancy { get; set; } = new();


        public List<EventSummary> UpcomingEvents { get; set; } = new();
        public List<NotificationSummary> Notifications { get; set; } = new();
        public List<QuickLink> QuickLinks { get; set; } = new();
    }
    // For Events
    public class EventSummary
    {
        public int EventId { get; set; }
        public string Title { get; set; } = "";
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
    }

    // For Notifications
    public class NotificationSummary
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    // Quick Links
    public class QuickLink
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string IconClass { get; set; } = "bi bi-link"; // Bootstrap Icons
    }

    public class ChartData
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
    }
}
