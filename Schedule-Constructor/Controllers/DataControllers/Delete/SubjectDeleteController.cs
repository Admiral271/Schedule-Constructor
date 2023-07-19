using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Delete
{
    public class SubjectDeleteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectDeleteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SubjectDelete(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == id);
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
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == id);
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
            return RedirectToAction("Subjects", "SubjectData");
        }
    }
}
