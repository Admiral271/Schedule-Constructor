using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Delete
{
    public class ScheduleDeleteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleDeleteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ScheduleDelete(int groupId, DateTime? startDate, DateTime? endDate)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ScheduleDeleteConfirmed(int groupId)
        {
            var schedulesToRemove = _context.Schedules.Where(s => s.GroupId == groupId).ToList();
            if (schedulesToRemove.Any())
            {
                _context.Schedules.RemoveRange(schedulesToRemove);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Расписание успешно удалено";
            }
            else
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Расписание не найдено";
            }
            return RedirectToAction("Schedules", "ScheduleData");
        }
    }
}
