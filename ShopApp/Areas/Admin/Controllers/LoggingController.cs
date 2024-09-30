using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Data;
using ShopApp.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ShopApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoggingController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public LoggingController(INotyfService toastNotification, ShopAppAspWebContext context, IMapper mapper, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username, Password")] LoginViewModel account, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accFound = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == account.Username && x.UserPassword == account.Password);

                if (accFound == null) {
                    _toastNotification.Error("Tài khoản hoặc mật khẩu không chính xác !", 3);
                    return View("Index", account);
                }
                else 
                {
                    if(accFound?.UserActive == 0)
                    {
                        _toastNotification.Error("Đăng nhập thất bại, tài khoản của bạn đã bị khóa !", 3);
                        return View("Index", account);
                    }
                    if (accFound?.UserRole == 1)
                    {
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim("userId", accFound.UserId.ToString()),
                            new Claim(ClaimTypes.Name, accFound.UserName),
                            new Claim("userFullName", accFound.UserFullName.ToString()),
                            new Claim(ClaimTypes.Email, accFound.UserEmail),
                            new Claim("avatar", accFound.UserAvatar ?? "default.png"),
                            new Claim(ClaimTypes.Role, accFound.UserRole == 1 ? "Admin" : "User"),
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);
                        var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        _toastNotification.Success("Đăng nhập thành công !", 3);
                        return Redirect("/Admin");
                    }

                    _toastNotification.Error("Đăng nhập thất bại, bạn không có quyền truy cập !", 3);

                    return View("Index", account);
                }
            }
            _toastNotification.Error("Đăng nhập thất bại !", 3);

            return View("Index", account);
        }

        [Authorize]
        public IActionResult Logout()
        {
            _toastNotification.Success("Đăng xuất thành công !", 3);
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (login.IsCompletedSuccessfully)
            {
                return RedirectToAction("Index", "Logging");
            }
            _toastNotification.Error("Đăng xuất thất bại !", 3);
            return RedirectToAction("Index", "Home");

        }

        private bool UserExists(int id)
        {
            return (_context.Accounts?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
