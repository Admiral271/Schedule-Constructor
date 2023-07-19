using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;
using Schedule_Constructor.Models;

namespace Schedule_Constructor.Controllers.DataControllers.Edit
{
    public class ScheduleEditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleEditController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ScheduleEdit(int groupId)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }

            var schedules = _context.Schedules.Include(s => s.Group).Include(s => s.Subject).Where(s => s.GroupId == groupId).ToList();
            ViewBag.Groups = _context.Groups.ToList();
            ViewBag.AllSubjects = _context.Subjects.ToList();
            ViewBag.SelectedGroup = group;
            return View(schedules);
        }

        [HttpPost]
        public IActionResult ScheduleEdit(int groupId, Dictionary<string, List<ScheduleData>> scheduleData)
        {
            if (scheduleData == null)
            {
                return RedirectToAction("Schedules", "ScheduleData");
            }

            var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Группа не найдена";
                return RedirectToAction("Schedules", "ScheduleData");
            }

            // Remove existing schedules for the selected group
            var schedulesToRemove = _context.Schedules.Where(s => s.GroupId == group.Id).ToList();
            _context.Schedules.RemoveRange(schedulesToRemove);

            // Add new schedules
            foreach (var dateScheduleData in scheduleData)
            {
                var date = DateTime.Parse(dateScheduleData.Key);
                foreach (var data in dateScheduleData.Value)
                {
                    if (data.SubjectId != 0)
                    {
                        var subject = _context.Subjects.FirstOrDefault(s => s.Id == data.SubjectId);

                        var schedule = new Schedule
                        {
                            GroupId = group.Id,
                            Group = group,
                            Date = date,
                            LessonNumber = data.LessonNumber,
                            SubjectId = subject?.Id ?? 0,
                            Subject = subject
                        };
                        _context.Schedules.Add(schedule);
                    }
                }
            }

            _context.SaveChanges();
            TempData["messageType"] = "success";
            TempData["message"] = "Расписание успешно отредактировано";
            return RedirectToAction("Schedules", "ScheduleData");
        }

        [HttpGet]
        public IActionResult CheckConflicts(int groupId, DateTime date, int lessonNumber, int subjectId)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == subjectId);

            // Check for conflicts with other groups
            var conflictingSchedules = _context.Schedules
                .Include(s => s.Group)
                .Where(s => s.Date == date && s.LessonNumber == lessonNumber && s.Subject.Teacher == subject.Teacher && s.GroupId != groupId)
                .ToList();

            if (conflictingSchedules.Count > 0)
            {
                // Generate a list of conflicting group names
                var conflictingGroupNames = string.Join(", ", conflictingSchedules.Select(s => s.Group.Name));

                // Generate an alert message
                return Json(new { hasConflicts = true, message = $"Преподаватель {subject.Teacher} ведёт занятие у другой группы ({conflictingGroupNames}) в это время" });
            }
            else
            {
                return Json(new { hasConflicts = false });
            }
        }
    }
}
