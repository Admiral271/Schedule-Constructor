using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Constructor.Data;

namespace Schedule_Constructor.Controllers.DataControllers.View
{
    public class ScheduleDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Schedules(int? groupId)
        {
            var groups = _context.Groups.ToList();
            ViewBag.Groups = groups;

            if (groupId == null)
            {
                return View();
            }

            var selectedGroup = groups.FirstOrDefault(g => g.Id == groupId);
            if (selectedGroup == null)
            {
                return NotFound();
            }

            ViewBag.SelectedGroup = selectedGroup;

            var schedules = _context.Schedules.Include(s => s.Group).Include(s => s.Subject).Where(s => s.GroupId == groupId).ToList();
            return View(schedules);
        }
    }
}
