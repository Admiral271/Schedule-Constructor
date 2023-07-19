using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;

namespace Schedule_Constructor.Controllers.DataControllers.Delete
{
    public class GroupDeleteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupDeleteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GroupDelete(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
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
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
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
            return RedirectToAction("Groups", "GroupData");
        }
    }
}
