using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



using SchoolManagementSystem.Models;

public class FooterLatestItemsViewComponent : ViewComponent
{
    private readonly SchoolDbContext _context;

    public FooterLatestItemsViewComponent(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int count = 5)
    {
        var student = await _context.Students
            .OrderByDescending(i => i.StudentId)
            .Take(count)
            .ToListAsync();

        return View(student);
    }
}
