using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Export
{
    public class PDFController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PDFController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ExportToPdf(int groupId)
        {
            // Получение данных расписания для группы
            var scheduleData = _context.Schedules.Where(s => s.GroupId == groupId).ToList();

            Console.WriteLine("Количество данных расписания для группы {0}: {1}", groupId, scheduleData.Count);
            foreach (var schedule in scheduleData)
            {
                Console.WriteLine("День недели: {0}, Номер пары: {1}, Предмет: {2}", schedule.DayOfWeek, schedule.LessonNumber, schedule.Subject?.Name);
            }

            // Проверка наличия данных расписания для группы
            if (!scheduleData.Any())
            {
                TempData["messageType"] = "error";
                TempData["message"] = "Нет данных расписания для выбранной группы";
                return RedirectToAction("Schedules", "ScheduleData");
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
                    var subjectName = schedule?.Subject?.Name ?? "";
                    Console.WriteLine("День недели: {0}, Номер пары: {1}, Предмет: {2}", dayOfWeek, lessonNumber, subjectName);
                }
                gfx.DrawString(daysOfWeek[dayOfWeek], font, XBrushes.Black, x + 10, y + (dayOfWeek + 1) * rowHeight + rowHeight / 2);
                for (int lessonNumber = 0; lessonNumber < 4; lessonNumber++)
                {
                    var schedule = scheduleData.FirstOrDefault(s => s.DayOfWeek == dayOfWeek && s.LessonNumber == lessonNumber);
                    var subjectName = schedule?.Subject?.Name ?? "";
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
    }
}
