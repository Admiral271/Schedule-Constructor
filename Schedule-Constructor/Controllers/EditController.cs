using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models;
using Schedule_Constructor.Models.DataModels;
using System.Linq;

namespace Schedule_Constructor.Controllers
{
    public class EditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EditController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Группы
        public IActionResult Groups()
        {
            var groups = _context.Groups.ToList();
            return View(groups);
        }

        public IActionResult GroupDetails(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }
        public IActionResult GroupEdit(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GroupEdit(int id, Group group)
        {
            if (id != group.Id_Group)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(group);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Группа успешно отредактирована";
                return RedirectToAction(nameof(Groups));
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при редактировании группы";
            return View(group);
        }

        public IActionResult GroupDelete(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost, ActionName("GroupDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult GroupDeleteConfirmed(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Группа успешно удалена";
            }
            else
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Группа не найдена";
            }
            return RedirectToAction(nameof(Groups));
        }

        // Предметы
        public IActionResult Subjects()
        {
            var subjects = _context.Subjects.ToList();
            return View(subjects);
        }

        public IActionResult SubjectDetails(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }
        public IActionResult SubjectEdit(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubjectEdit(int id, Subject subject)
        {
            if (id != subject.Id_Subject)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(subject);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Предмет успешно отредактирован";
                return RedirectToAction(nameof(Subjects));
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при редактировании предмета";
            return View(subject);
        }


        public IActionResult SubjectDelete(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        [HttpPost, ActionName("SubjectDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult SubjectDeleteConfirmed(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Предмет успешно удален";
            }
            else
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Предмет не найден";
            }
            return RedirectToAction(nameof(Subjects));
        }


        // Расписания
        public IActionResult Schedules(int? groupId)
        {
            var groups = _context.Groups.ToList();
            ViewBag.Groups = groups;

            if (groupId == null)
            {
                return View();
            }

            var selectedGroup = groups.FirstOrDefault(g => g.Id_Group == groupId);
            if (selectedGroup == null)
            {
                return NotFound();
            }

            ViewBag.SelectedGroup = selectedGroup;

            var schedules = _context.Schedules.Include(s => s.Group).Include(s => s.Subject).Where(s => s.GroupId == groupId).ToList();
            return View(schedules);
        }

        public IActionResult ScheduleEdit(int groupId)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == groupId);
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
        [ValidateAntiForgeryToken]
        public IActionResult ScheduleEdit(int groupId, ScheduleViewModel scheduleViewModel)
        {
            if (ModelState.IsValid)
            {
                var group = _context.Groups.FirstOrDefault(g => g.Id_Group == groupId);
                if (group == null)
                {
                    TempData["messageType"] = "error";
                    TempData["message"] = "Группа не найдена";
                    return RedirectToAction("Schedules", "Edit");
                }

                var schedulesToRemove = _context.Schedules.Where(s => s.GroupId == group.Id_Group).ToList();
                _context.Schedules.RemoveRange(schedulesToRemove);

                foreach (var scheduleData in scheduleViewModel.ScheduleData)
                {
                    var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == scheduleData.SubjectId);
                    if (subject == null)
                    {
                        continue;
                    }

                    var schedule = new Schedule
                    {
                        GroupId = group.Id_Group,
                        Group = group,
                        DayOfWeek = scheduleData.DayOfWeek,
                        LessonNumber = scheduleData.LessonNumber,
                        SubjectId = subject.Id_Subject,
                        Subject = subject
                    };
                    _context.Schedules.Add(schedule);
                }
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Расписание успешно отредактировано";
                return RedirectToAction("Schedules", "Edit");
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при редактировании расписания";
            return RedirectToAction("Schedules", "Edit");
        }


        public IActionResult ScheduleDelete(int groupId)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == groupId);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost, ActionName("ScheduleDelete")]
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
            return RedirectToAction(nameof(Schedules));
        }


    }
}
