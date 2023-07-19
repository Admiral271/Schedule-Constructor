using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;

namespace Schedule_Constructor.Controllers.DataControllers.View
{
    public class SubjectDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Subjects()
        {
            var subjects = _context.Subjects.ToList();
            return View(subjects);
        }
    }
}
