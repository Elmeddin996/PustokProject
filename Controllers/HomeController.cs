using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;
using System.Diagnostics;

namespace PustokProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        public HomeController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel
            {
                FeaturedBoooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsFeatured).Take(10).ToList(),
                NewBoooks = _context.Books.Include("Author").Include(x => x.BookImages).Where(x => x.IsNew).Take(10).ToList(),
                DiscountedBoooks = _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.DiscountPercent > 0).Take(10).ToList(),
            };

            return View(vm);
        }

    }
}