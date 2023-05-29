using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokProject.DAL;
using P328Pustok.Models;
using PustokProject.ViewModels;
using PustokProject.Models;
using PustokProject.ViewModels;
using System.Security.Claims;
using PustokProject.DAL.DataAccess;

namespace PustokProject.Controllers
{
    public class BookController : Controller
    {
        private readonly DataContext _context;

        public BookController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            Book book = _context.Books
                .Include(x => x.BookImages)
                .Include(x => x.Author)
                .Include(x => x.Genre)
                .Include(x => x.BookComments).ThenInclude(x => x.AppUser)
                .Include(x => x.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(x => x.Id == id);

            if (book == null) return View("Error");

            BookDetailViewModel vm = new BookDetailViewModel
            {
                Book = book,
                RelatedBooks = _context.Books.Include(x => x.BookImages).Where(x => x.GenreId == book.GenreId).ToList(),
                Comment = new BookComment { BookId = id }
            };



            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comment(BookComment comment)
        {

            if (!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
                return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = comment.BookId }) });


            if (!ModelState.IsValid)
            {
                Book book = _context.Books
            .Include(x => x.BookImages)
            .Include(x => x.Author)
            .Include(x => x.Genre)
                .Include(x => x.BookComments).ThenInclude(x => x.AppUser)
            .Include(x => x.BookTags).ThenInclude(bt => bt.Tag).FirstOrDefault(x => x.Id == comment.BookId);

                if (book == null) return View("Error");

                BookDetailViewModel vm = new BookDetailViewModel
                {
                    Book = book,
                    RelatedBooks = _context.Books.Include(x => x.BookImages).Where(x => x.GenreId == book.GenreId).ToList(),
                    Comment = new BookComment { BookId = comment.BookId }
                };
                vm.Comment = comment;
                return View("Detail", vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            comment.AppUserId = userId;
            comment.CreatedAt = DateTime.UtcNow.AddHours(4);

            _context.BookComments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("detail", new { id = comment.BookId });

        }


        public IActionResult GetBookDetail(int id)
        {
            Book book = _context.Books
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .Include(x => x.BookTags).ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);

            if (book == null) return StatusCode(404);

            //return Json(new { book = book });
            return PartialView("_BookModalPartial", book);
        }

        public IActionResult AddToBasket(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var basketItem = _context.BasketItems.FirstOrDefault(x => x.BookId == id && x.AppUserId == userId);
                if (basketItem != null) basketItem.Count++;
                else
                {
                    basketItem = new BasketItem { AppUserId = userId, BookId = id, Count = 1 };
                    _context.BasketItems.Add(basketItem);
                }
                _context.SaveChanges();
                var basketItems = _context.BasketItems.Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x => x.AppUserId == userId).ToList();


                return PartialView("_BasketPartialView", GenerateBasketVM(basketItems));
            }
            else
            {
                List<BasketItemCookieViewModel> cookieItems = new List<BasketItemCookieViewModel>();

                BasketItemCookieViewModel cookieItem;
                var basketStr = Request.Cookies["basket"];
                if (basketStr != null)
                {
                    cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

                    cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);

                    if (cookieItem != null)
                        cookieItem.Count++;
                    else
                    {
                        cookieItem = new BasketItemCookieViewModel { BookId = id, Count = 1 };
                        cookieItems.Add(cookieItem);
                    }
                }
                else
                {
                    cookieItem = new BasketItemCookieViewModel { BookId = id, Count = 1 };
                    cookieItems.Add(cookieItem);
                }

                Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));
                return PartialView("_BasketPartialView", GenerateBasketVM(cookieItems));
            }
        }

        private BasketViewModel GenerateBasketVM(List<BasketItemCookieViewModel> cookieItems)
        {
            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId)
                };
                bv.Items.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
            }

            return bv;
        }

        private BasketViewModel GenerateBasketVM(List<BasketItem> basketItems)
        {
            BasketViewModel bv = new BasketViewModel();
            foreach (var item in basketItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = item.Count,
                    Book = item.Book
                };
                bv.Items.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
            }
            return bv;
        }


        public IActionResult RemoveBasket(int id)
        {
            var basketStr = Request.Cookies["basket"];
            if (basketStr == null)
                return StatusCode(404);

            List<BasketItemCookieViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

            BasketItemCookieViewModel item = cookieItems.FirstOrDefault(x => x.BookId == id);

            if (item == null)
                return StatusCode(404);

            if (item.Count > 1)
                item.Count--;
            else
                cookieItems.Remove(item);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId)
                };
                bv.Items.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPercent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPercent) / 100) : bi.Book.SalePrice) * bi.Count;
            }

            return PartialView("_BasketPartialView", bv);
        }

        public IActionResult ShowBasket()
        {
            var basket = new List<BasketItemCookieViewModel>();
            var basketStr = Request.Cookies["basket"];

            if (basketStr != null)
                basket = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

            return Json(new { basket });
        }
    }
}