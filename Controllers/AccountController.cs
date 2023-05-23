using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokProject.Models;
using PustokProject.ViewModels;

namespace PustokProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            if (_userManager.Users.Any(x => x.UserName == registerVM.UserName))
            {
                ModelState.AddModelError("UserName", "UserName is alredy taken");
                return View();
            }

            if (_userManager.Users.Any(x => x.Email == registerVM.Email))
            {
                ModelState.AddModelError("Email", "Email is alredy taken");
                return View();
            }

            AppUser user = new AppUser
            {
                FullName = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

    }
}
