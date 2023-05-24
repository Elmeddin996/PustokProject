using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.ViewModels;
using System.Security.Claims;

namespace PustokProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(DataContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Checkout()
        {
            OrderViewModel vm = new OrderViewModel();
            vm.CheckoutItems = GenerateCheckoutItems();

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                vm.Order = new OrderCreateViewModel
                {
                    Address = user.Adress,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone
                };
            }

            vm.TotalPrice = vm.CheckoutItems.Any() ? vm.CheckoutItems.Sum(x => x.Price * x.Count) : 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel orderVM)
        {
            if (!ModelState.IsValid)
            {
                OrderViewModel vm = new OrderViewModel();
                vm.CheckoutItems = GenerateCheckoutItems();
                vm.Order = orderVM;
                return View("Checkout", vm);
            }


            //order create

            return Json(orderVM);
        }

        private List<CheckoutItem> GenerateCheckoutItemsFromDb(string userId)
        {
            return _context.BasketItems.Include(x => x.Book).Where(x => x.AppUser.Id == userId).Select(x => new CheckoutItem
            {
                Count = x.Count,
                Name = x.Book.Name,
                Price = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice
            }).ToList();
        }

        private List<CheckoutItem> GenerateCheckoutItemsFromCookie()
        {
            List<CheckoutItem> checkoutItems = new List<CheckoutItem>();
            var basketStr = Request.Cookies["basket"];

            if (basketStr != null)
            {
                List<BasketItemCookieViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

                foreach (var item in cookieItems)
                {
                    Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);

                    CheckoutItem checkoutItem = new CheckoutItem
                    {
                        Count = item.Count,
                        Name = book.Name,
                        Price = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice
                    };
                    checkoutItems.Add(checkoutItem);
                }
            }

            return checkoutItems;
        }

        public List<CheckoutItem> GenerateCheckoutItems()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return GenerateCheckoutItemsFromDb(userId);
            }
            else
                return GenerateCheckoutItemsFromCookie();
        }


    }
}
