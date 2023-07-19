using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Edit
{
    public class GroupEditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupEditController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GroupEdit(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
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
            if (id != group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(group);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Группа успешно отредактирована";
                return RedirectToAction("Groups", "GroupData");
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при редактировании группы";
            return View(group);
        }
    }
}
