using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly DataContext _context;

        public AuthorController(DataContext context)
        {
            _context = context;
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
        public IActionResult Create(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
