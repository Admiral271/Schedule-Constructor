using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;
using Schedule_Constructor.Models.DataModels;

namespace Schedule_Constructor.Controllers.DataControllers.Edit
{
    public class SubjectEditController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectEditController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult SubjectEdit(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.Id == id);
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
            if (id != subject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(subject);
                _context.SaveChanges();
                TempData["messageType"] = "success";
                TempData["message"] = "Предмет успешно отредактирован";
                return RedirectToAction("Subjects", "SubjectData");
            }
            TempData["messageType"] = "error";
            TempData["message"] = "Ошибка при редактировании предмета";
            return View(subject);
        }
    }
}
