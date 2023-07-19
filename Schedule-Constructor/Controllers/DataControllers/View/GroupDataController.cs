using Microsoft.AspNetCore.Mvc;
using Schedule_Constructor.Data;

namespace Schedule_Constructor.Controllers.DataControllers.View
{
    public class GroupDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupDataController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Groups()
        {
            var groups = _context.Groups.ToList();
            return View(groups);
        }
    }
}
