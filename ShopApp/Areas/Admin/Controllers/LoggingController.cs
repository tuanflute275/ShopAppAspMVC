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
using System.Text;

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
        public async Task<IActionResult> Login([Bind("Username, UserPassword")] LoginViewModel account, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var accFound = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == account.Username);
                if (accFound != null) {
                    if (accFound?.UserActive == 1)
                    {
                        if (accFound.UserPassword == CreateMD5(account.UserPassword)) {
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
                            else
                            {
                                _toastNotification.Error("Bạn không có quyền truy cập !", 3);
                                return View("Index", account);
                            }
                        }
                        else
                        {
                            _toastNotification.Error("Mật khẩu không chính xác !", 3);
                            return View("Index", account);
                        }
                    }
                    else
                    {
                        _toastNotification.Error("Tài khoản của bạn đã bị khóa !", 3);
                        return View("Index", account);
                    }
                }
                else 
                {
                    _toastNotification.Error("Tài khoản Không tồn tại !", 3);
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

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
