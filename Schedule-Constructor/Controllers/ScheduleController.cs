using Schedule_Constructor.Data;
using Schedule_Constructor.Models;
using Schedule_Constructor.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Schedule_Constructor.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ScheduleController(ApplicationDbContext context)
        {
            applicationDbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Конструктор";

            var groups = applicationDbContext.Groups.ToList();
            ViewBag.Groups = groups;

            // Получение списка всех предметов
            var allSubjects = applicationDbContext.Subjects.ToList();
            ViewBag.AllSubjects = allSubjects;

            return View();
        }

        [HttpPost]
        public IActionResult Schedule(int group, DateTime startDate, DateTime endDate)
        {
            // Получение группы из базы данных
            var selectedGroup = applicationDbContext.Groups.FirstOrDefault(g => g.Id_Group == group);

            // Получение списка всех предметов
            var allSubjects = applicationDbContext.Subjects.ToList();

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
            var groups = applicationDbContext.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Group)
                .Where(s => s.Date == date && s.LessonNumber == lessonNumber && s.Subject.Teacher == teacher)
                .Select(s => s.Group.Name_Group)
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
        [Route("/Schedule/Save")]
        public IActionResult SaveSchedule([FromBody] ScheduleViewModel data)
        {
            if (data == null || data.ScheduleData == null)
            {
                return RedirectToAction("Index");
            }

            // Удаление существующего расписания для выбранной группы
            var schedulesToRemove = applicationDbContext.Schedules.Where(s => s.GroupId == data.Group).ToList();
            applicationDbContext.Schedules.RemoveRange(schedulesToRemove);

            // Добавление новых данных расписания для выбранной группы
            foreach (var scheduleData in data.ScheduleData)
            {
                // Проверка, равен ли SubjectId нулю
                if (scheduleData.SubjectId != 0)
                {
                    var schedule = new Schedule
                    {
                        GroupId = data.Group,
                        Date = DateTime.Parse(scheduleData.Date),
                        LessonNumber = scheduleData.LessonNumber,
                        SubjectId = scheduleData.SubjectId
                    };
                    applicationDbContext.Schedules.Add(schedule);
                }
            }
            applicationDbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
