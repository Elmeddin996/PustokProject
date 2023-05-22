using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Helpers;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        public AuthorController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            var query = _context.Authors.Include(x=>x.Books).AsQueryable();

            if (search != null)
                query = query.Where(x => x.FullName.Contains(search));

            ViewBag.Search = search;

            return View(PaginatedList<Author>.Create(query, page, 1));
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid) return View();

            

            author.ImageName = FileManager.Save(_env.WebRootPath, "uploads/authors", author.AutorImageFile);
            


            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.Find(id);

            if (author == null) return StatusCode(404);

            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {
            if (!ModelState.IsValid) return View();

            Author existAuthor = _context.Authors.Find(author.Id);

            if (existAuthor == null) return StatusCode(404);

            if (author.FullName != existAuthor.FullName && _context.Authors.Any(x => x.FullName == author.FullName))
            {
                ModelState.AddModelError("Name", "Name is already taken");
                return View();
            }

            existAuthor.FullName = author.FullName;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.Find(id);

            if (author == null) return StatusCode(404);

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
