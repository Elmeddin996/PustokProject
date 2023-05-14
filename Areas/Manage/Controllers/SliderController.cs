using Microsoft.AspNetCore.Mvc;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SliderController : Controller
    {
        private readonly DataContext _context;

        public SliderController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            var query = _context.Sliders.AsQueryable();

            if (search != null)
                query = query.Where(x => x.Title.Contains(search));

            ViewBag.Search = search;

            return View(PaginatedList<Slider>.Create(query, page, 1));
        }
    }
}
