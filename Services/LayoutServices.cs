using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Services
{
    public class LayoutServices
    {
        private readonly DataContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutServices(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
           _httpContextAccessor= httpContextAccessor;
        }
        public Dictionary<string,string> GetSettings()
        {
         
            return _context.Settings.ToDictionary(x=>x.Key,x=>x.Value);
        }

        public List<Genre> GetGenres() 
        {
            return _context.Genres.ToList();
        }

        public BasketViewModel GetBasket()
        {
            var bv =new BasketViewModel();
            var basketJson = _httpContextAccessor.HttpContext.Request.Cookies["basket"];
            if (basketJson != null)
            {
                var cookieItems =JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketJson);

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

            }
            return bv;  
        }
    }
}
