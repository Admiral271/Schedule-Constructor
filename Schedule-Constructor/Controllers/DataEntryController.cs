using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers
{
    public class DataEntryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataEntryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Subjects()
        {
            return View();
        }

        public IActionResult Groups()
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

        [HttpPost]
        public async Task<IActionResult> CreateGroup(Group group)
        {
            if (ModelState.IsValid)
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                TempData["messageType"] = "success";
                TempData["message"] = "Группа успешно создана";
                return RedirectToAction(nameof(Groups));
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при создании группы";
            return View(group);
        }


    }
}
