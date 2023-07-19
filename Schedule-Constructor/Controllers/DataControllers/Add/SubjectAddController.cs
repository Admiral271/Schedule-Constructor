using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Add
{
    public class SubjectAddController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectAddController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Subjects()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                TempData["messageType"] = "success";
                TempData["message"] = "Предмет успешно создан";
                return RedirectToAction(nameof(Subjects));
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при создании предмета";
            return View(subject);
        }
    }
}
