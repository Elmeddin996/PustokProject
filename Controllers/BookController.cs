using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;


namespace PustokProject.Controllers
{
    public class BookController : Controller
    {

        private readonly DataContext _context;

        public BookController(DataContext context)
        {
            _context = context;
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
            return PartialView("_BookModelPartial", book);
        }


        public IActionResult AddToBasket(int id)
        {
            List<BasketItemCookieViewModel> cookieItems = new List<BasketItemCookieViewModel>();

            BasketItemCookieViewModel cookieItem;

            var basketStr = Request.Cookies["basket"];

            if (basketStr != null)
            {
                cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

                cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);

                if (cookieItem != null)
                {
                    cookieItem.Count++;
                }

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

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();

            foreach (var ci in cookieItems) 
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Book = _context.Books.FirstOrDefault(x => x.Id == ci.BookId)
                };
                basketItems.Add(bi);
            }

            return PartialView("_BasketPartialView",basketItems);
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

