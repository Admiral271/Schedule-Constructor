using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models;
using Schedule_Constructor.Models.DataModels;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;



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
        public IActionResult ScheduleEdit(int groupId, List<ScheduleData> scheduleData)
        {
            if (scheduleData == null)
            {
                return RedirectToAction("Schedules", "Edit");
            }

            var group = _context.Groups.FirstOrDefault(g => g.Id_Group == groupId);
            if (group == null)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Группа не найдена";
                return RedirectToAction("Schedules", "Edit");
            }

            var schedulesToRemove = _context.Schedules.Where(s => s.GroupId == group.Id_Group).ToList();
            _context.Schedules.RemoveRange(schedulesToRemove);

            foreach (var data in scheduleData)
            {
                if (data.SubjectId != 0)
                {
                    var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == data.SubjectId);

                    var schedule = new Schedule
                    {
                        GroupId = group.Id_Group,
                        Group = group,
                        DayOfWeek = data.DayOfWeek,
                        LessonNumber = data.LessonNumber,
                        SubjectId = subject?.Id_Subject ?? 0,
                        Subject = subject
                    };
                    _context.Schedules.Add(schedule);
                }
            }

            _context.SaveChanges();
            TempData["messageType"] = "success";
            TempData["message"] = "Расписание успешно отредактировано";
            return RedirectToAction("Schedules", "Edit");
        }


        [HttpGet]
        public IActionResult CheckConflicts(int groupId, int dayOfWeek, int lessonNumber, int subjectId)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id_Subject == subjectId);

            // Check for conflicts with other groups
            var conflictingSchedules = _context.Schedules
                .Include(s => s.Group)
                .Where(s => s.DayOfWeek == dayOfWeek && s.LessonNumber == lessonNumber && s.Subject.Teacher == subject.Teacher && s.GroupId != groupId)
                .ToList();

            if (conflictingSchedules.Count > 0)
            {
                // Generate a list of conflicting group names
                var conflictingGroupNames = string.Join(", ", conflictingSchedules.Select(s => s.Group.Name_Group));

                // Generate an alert message
                return Json(new { hasConflicts = true, message = $"Преподаватель {subject.Teacher} ведёт занятие у другой группы ({conflictingGroupNames}) в это время" });
            }
            else
            {
                return Json(new { hasConflicts = false });
            }
        }

        public async Task<IActionResult> ExportToPdf(int groupId)
        {
            // Получение данных расписания для группы
            var scheduleData = _context.Schedules.Where(s => s.GroupId == groupId).ToList();

            Console.WriteLine("Количество данных расписания для группы {0}: {1}", groupId, scheduleData.Count);
            foreach (var schedule in scheduleData)
            {
                Console.WriteLine("День недели: {0}, Номер пары: {1}, Предмет: {2}", schedule.DayOfWeek, schedule.LessonNumber, schedule.Subject?.Name_Subject);
            }

            // Проверка наличия данных расписания для группы
            if (!scheduleData.Any())
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Нет данных расписания для выбранной группы";
                return RedirectToAction(nameof(Schedules));
            }

            // Создание нового PDF-документа
            var pdfDoc = new PdfDocument();
            var pdfPage = pdfDoc.AddPage();
            var gfx = XGraphics.FromPdfPage(pdfPage);
            var font = new XFont("Arial", 14, XFontStyle.Bold);

            // Определение размеров и координат таблицы
            int colCount = 5;
            int rowCount = 7;
            int colWidth = 100;
            int rowHeight = 30;
            int tableWidth = colCount * colWidth;
            int tableHeight = rowCount * rowHeight;
            int x = (int)(pdfPage.Width / 2) - (tableWidth / 2);
            int y = (int)(pdfPage.Height / 2) - (tableHeight / 2);

            // Рисование таблицы
            for (int i = 0; i <= colCount; i++)
            {
                gfx.DrawLine(XPens.Black, x + i * colWidth, y, x + i * colWidth, y + tableHeight);
            }
            for (int i = 0; i <= rowCount; i++)
            {
                gfx.DrawLine(XPens.Black, x, y + i * rowHeight, x + tableWidth, y + i * rowHeight);
            }

            // Заполнение заголовков таблицы
            string[] headers = new string[] { "День недели", "1 пара", "2 пара", "3 пара", "4 пара" };
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawString(headers[i], font, XBrushes.Black, x + i * colWidth + 10, y + rowHeight / 2);
            }
            // Заполнение данных таблицы
            string[] daysOfWeek = new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            for (int dayOfWeek = 0; dayOfWeek < daysOfWeek.Length; dayOfWeek++)
            {
                for (int lessonNumber = 0; lessonNumber < 4; lessonNumber++)
                {
                    var schedule = scheduleData.FirstOrDefault(s => s.DayOfWeek == dayOfWeek && s.LessonNumber == lessonNumber);
                    var subjectName = schedule?.Subject?.Name_Subject ?? "";
                    Console.WriteLine("День недели: {0}, Номер пары: {1}, Предмет: {2}", dayOfWeek, lessonNumber, subjectName);
                }
                gfx.DrawString(daysOfWeek[dayOfWeek], font, XBrushes.Black, x + 10, y + (dayOfWeek + 1) * rowHeight + rowHeight / 2);
                for (int lessonNumber = 0; lessonNumber < 4; lessonNumber++)
                {
                    var schedule = scheduleData.FirstOrDefault(s => s.DayOfWeek == dayOfWeek && s.LessonNumber == lessonNumber);
                    var subjectName = schedule?.Subject?.Name_Subject ?? "";
                    gfx.DrawString(subjectName, font, XBrushes.Black, x + (lessonNumber + 1) * colWidth + 10, y + (dayOfWeek + 1) * rowHeight + rowHeight / 2);
                }
            }

            // Сохранение PDF-документа в поток
            using (var ms = new MemoryStream())
            {
                pdfDoc.Save(ms);

                // Отправка PDF-файла пользователю
                Response.ContentType = "application/pdf";
                Response.Headers.Add("Content-Disposition", "attachment; filename=schedule.pdf");
                await Response.Body.WriteAsync(ms.ToArray());
            }

            return new EmptyResult();
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
