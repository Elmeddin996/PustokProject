using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Helpers;
using PustokProject.Models;
using PustokProject.ViewModels;
using System.Data;

namespace PustokProject.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly DataContext _context;

        public IWebHostEnvironment _env;

        public BookController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            var query = _context.Books
                .Include(x => x.Author).Include(x => x.Genre).Include(x => x.BookImages.Where(bi => bi.PosterStatus == true)).AsQueryable();

            if (search != null)
                query = query.Where(x => x.Name.Contains(search));

            ViewBag.Search = search;

            return View(PaginatedList<Book>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();



            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid) return View();

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "AuthorIs is not correct");
                return View();
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "GenreId is not correct");
                return View();
            }


            if (book.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "PosterImage is required");
                return View();
            }
            if (book.HoverPosterImage == null)
            {
                ModelState.AddModelError("HoverPosterImage", "HoverPosterImage is required");
                return View();
            }

            foreach (var tagId in book.TagIds)
            {
                BookTag bookTag = new BookTag
                {
                    TagId = tagId,
                };

                book.BookTags.Add(bookTag);
            }

            BookImage poster = new BookImage
            {
                ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage),
                PosterStatus = true,
            };
            book.BookImages.Add(poster);

            BookImage hoverPoster = new BookImage
            {
                ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImage),
                PosterStatus = false,
            };
            book.BookImages.Add(hoverPoster);

            foreach (var img in book.Images)
            {
                BookImage bookImage = new BookImage
                {
                    ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", img),
                };
                book.BookImages.Add(bookImage);
            }



            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();


            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid) return View();

            Book existBook = _context.Books.Include(x => x.BookTags).Include(x => x.BookImages).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null) return View("Error");

            if (book.AuthorId != existBook.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "AuthorIs is not correct");
                return View();
            }

            if (book.GenreId != existBook.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "GenreId is not correct");
                return View();
            }

            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));

            var newTagIds = book.TagIds.FindAll(x => !existBook.BookTags.Any(bt => bt.TagId == x));
            foreach (var tagId in newTagIds)
            {
                BookTag bookTag = new BookTag { TagId = tagId };
                existBook.BookTags.Add(bookTag);
            }


            string oldPoster = null;
            if (book.PosterImage != null)
            {
                BookImage poster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                oldPoster = poster?.ImageName;

                if (poster == null)
                {
                    poster = new BookImage { PosterStatus = true };
                    poster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage);
                    existBook.BookImages.Add(poster);
                }
                else
                    poster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage);
            }

            string oldHoverPoster = null;
            if (book.HoverPosterImage != null)
            {
                BookImage hoverPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                oldHoverPoster = hoverPoster?.ImageName;

                if (hoverPoster == null)
                {
                    hoverPoster = new BookImage { PosterStatus = false };
                    hoverPoster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImage);
                    existBook.BookImages.Add(hoverPoster);
                }
                else
                    hoverPoster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImage);
            }

            var removedImages = existBook.BookImages.FindAll(x => x.PosterStatus == null && !book.BookImageIds.Contains(x.Id));
            existBook.BookImages.RemoveAll(x => x.PosterStatus == null && !book.BookImageIds.Contains(x.Id));

            foreach (var item in book.Images)
            {
                BookImage bookImage = new BookImage
                {
                    ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                };
                existBook.BookImages.Add(bookImage);
            }

            existBook.Name = book.Name;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.Desc = book.Desc;
            existBook.IsFeatured = book.IsFeatured;
            existBook.IsNew = book.IsNew;
            existBook.StockStatus = book.StockStatus;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.AuthorId = book.AuthorId;
            existBook.GenreId = book.GenreId;

            _context.SaveChanges();


            if (oldPoster != null) FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster);
            if (oldHoverPoster != null) FileManager.Delete(_env.WebRootPath, "uploads/books", oldHoverPoster);

            if (removedImages.Any())
                FileManager.DeleteAll(_env.WebRootPath, "uploads/books", removedImages.Select(x => x.ImageName).ToList());


            return RedirectToAction("index");
        }


        public IActionResult Delete (int id)
        {
            Book book =_context.Books.Find(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }


}
