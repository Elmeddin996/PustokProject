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

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.Find(id);

            if (slider == null) return View("Error");

            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            Slider existSlider = _context.Sliders.Find(slider.Id);

            if (existSlider == null) return View("Error");

            if (slider.Title != slider.Title && _context.Sliders.Any(x => x.Title == slider.Title))
            {
                ModelState.AddModelError("Title", "Title is already taken");
                return View();
            }

            existSlider.Title = slider.Title;

            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
