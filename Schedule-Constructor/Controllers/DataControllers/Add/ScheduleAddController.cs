using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;
using Schedule_Constructor.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Schedule_Constructor.Controllers.DataControllers.Add
{
    public class ScheduleAddController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleAddController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Конструктор";

            var groups = _context.Groups.ToList();
            ViewBag.Groups = groups;

            // Получение списка всех предметов
            var allSubjects = _context.Subjects.ToList();
            ViewBag.AllSubjects = allSubjects;

            return View();
        }

        [HttpPost]
        public IActionResult Schedule(int group, DateTime startDate, DateTime endDate)
        {
            // Получение группы из базы данных
            var selectedGroup = _context.Groups.FirstOrDefault(g => g.Id == group);

            // Получение списка всех предметов
            var allSubjects = _context.Subjects.ToList();

            // Передача данных в представление
            ViewBag.Group = selectedGroup;
            ViewBag.AllSubjects = allSubjects;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View("Index");
        }

        [HttpGet]
        public IActionResult CheckTeacherAvailability(DateTime date, int lessonNumber, string teacher)
        {
            // Получите список всех групп, которым назначен выбранный преподаватель в то же время
            var groups = _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Group)
                .Where(s => s.Date == date && s.LessonNumber == lessonNumber && s.Subject.Teacher == teacher)
                .Select(s => s.Group.Name)
                .ToList();

            if (groups.Count > 0)
            {
                return Json(new { isAvailable = false, groups = groups });
            }
            else
            {
                return Json(new { isAvailable = true });
            }
        }

        [HttpPost]
        [Route("/ScheduleAdd/Save")]
        public IActionResult SaveSchedule([FromBody] ScheduleViewModel data)
        {
            if (data == null || data.ScheduleData == null)
            {
                return RedirectToAction("Index");
            }

            // Удаление существующего расписания для выбранной группы
            var schedulesToRemove = _context.Schedules.Where(s => s.GroupId == data.Group).ToList();
            _context.Schedules.RemoveRange(schedulesToRemove);

            // Добавление новых данных расписания для выбранной группы
            foreach (var scheduleData in data.ScheduleData)
            {
                // Проверка, равен ли SubjectId нулю
                if (scheduleData.SubjectId != 0)
                {
                    var date = DateTime.Parse(scheduleData.Date);
                    var calendar = CultureInfo.InvariantCulture.Calendar;
                    var weekOfYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    var schedule = new Schedule
                    {
                        GroupId = data.Group,
                        Date = date,
                        LessonNumber = scheduleData.LessonNumber,
                        SubjectId = scheduleData.SubjectId,
                        IsEvenWeek = weekOfYear % 2 == 0 
                    };
                    _context.Schedules.Add(schedule);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "ScheduleAdd");
        }
    }
}
