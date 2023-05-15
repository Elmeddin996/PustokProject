using Microsoft.AspNetCore.Mvc;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly DataContext _context;

        public GenreController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            var query = _context.Genres.AsQueryable();

            if (search != null)
                query = query.Where(x => x.Name.Contains(search));

            ViewBag.Search = search;

            return View(PaginatedList<Genre>.Create(query, page, 1));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.Find(id);

            if (genre == null) return View("Error");

            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            if (!ModelState.IsValid) return View();

            Genre existGenre = _context.Genres.Find(genre.Id);

            if (existGenre == null) return View("Error");

            if (genre.Name != existGenre.Name && _context.Genres.Any(x => x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken");
                return View();
            }

            existGenre.Name = genre.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.Find(id);

            if (genre == null) return StatusCode(404);

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
