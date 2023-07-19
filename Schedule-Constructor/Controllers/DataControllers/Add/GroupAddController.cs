using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Add
{
    public class GroupAddController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupAddController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Groups()
        {
            return View();
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
