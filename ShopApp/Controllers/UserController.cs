using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Data;
using ShopApp.Models.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace ShopApp.Controllers
{

    public class UserController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public UserController(ShopAppAspWebContext context, INotyfService notyfService, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _context = context;
            _toastNotification = notyfService;
            _environment = environment;
        }


        [HttpGet]
        [Route("quen-mat-khau")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("quen-mat-khau")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                if (AccountEmailExists(forgotPasswordModel.UserEmail))
                {
                    var acc = _context.Accounts.FirstOrDefault(x => x.UserEmail == forgotPasswordModel.UserEmail);
                    var pass = CreateRandomPassword(8);
                    acc.UserPassword = pass;
                    _context.Update(acc);
                    await _context.SaveChangesAsync();
                    EmailModel model = new EmailModel()
                    {
                        Subject = "Thay đổi mật khẩu",
                        To = forgotPasswordModel.UserEmail
                    };
                    using (MailMessage mm = new MailMessage(model.From, model.To))
                    {
                        mm.Subject = model.Subject;
                        mm.Body = BodyResetPasswordMail(pass);
                        mm.IsBodyHtml = true;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            NetworkCredential NetworkCred = new NetworkCredential(model.From, model.Password);
                            smtp.UseDefaultCredentials = false;
                            smtp.EnableSsl = true;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                    TempData["message"] = "Đăng ký tài khoản thành công, vui lòng kiểm tra email!";
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    TempData["error"] = "Tài khoản không tồn tại!";
                    return View(forgotPasswordModel);
                }
            }
            return View(forgotPasswordModel);
        }

            [HttpGet]
        [Route("dang-nhap")]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("dang-nhap")]
        public IActionResult Login(LoginViewModel model, string? returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var checkAccount = _context.Accounts.FirstOrDefault(a => a.UserName == model.Username);
                if (checkAccount != null)
                {
                    HttpContext.Session.SetInt32("customerID", checkAccount.UserId);
                    if (checkAccount.UserActive == 1)
                    {
                        if (checkAccount.UserPassword == CreateMD5(model.UserPassword))
                        {
                            _toastNotification.Success("Đăng nhập thành công!");
                            if (Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                            else
                                return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["error"] = "Sai mật khẩu!";
                            return View(model);
                        }
                    }
                    else
                    {
                        TempData["error"] = "Tài khoản đã bị khóa!";
                        return View(model);
                    }
                }
                else
                {
                    TempData["error"] = "Tài khoản không tồn tại!";
                    return View(model);
                }
            }
        }

        [HttpGet]
        [Route("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("dang-ky")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account account)
        {
            // Validate data
            try
            {
                if (string.IsNullOrWhiteSpace(account.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản không được bỏ trống!");
                }
                else if (AccountUserNameExists(account.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại trong hệ thống!");
                }

                if (string.IsNullOrWhiteSpace(account.UserFullName))
                {
                    ModelState.AddModelError("UserFullName", "Tên đầy đủ không được bỏ trống!");
                }

                if (string.IsNullOrWhiteSpace(account.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email không được bỏ trống!");
                }
                else if (AccountEmailExists(account.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email đã tồn tại trong hệ thống!");
                }

                if (string.IsNullOrWhiteSpace(account.UserPassword))
                {
                    ModelState.AddModelError("UserPassword", "Mật khẩu không được bỏ trống!");
                }
                else if (account.UserPassword.Length < 6)
                {
                    ModelState.AddModelError("UserPassword", "Mật khẩu tối thiểu 6 ký tự!");
                }

                // Nếu ModelState không hợp lệ, trả lại view với dữ liệu lỗi
                if (!ModelState.IsValid)
                {
                    return View(account);
                }

                account.UserRole = 0;
                account.UserActive = 1;
                account.UserPassword = CreateMD5(account.UserPassword);

                _context.Add(account);
                await _context.SaveChangesAsync();

                // Gửi email xác nhận đăng ký
                EmailModel model = new EmailModel()
                {
                    Subject = "Đăng ký tài khoản",
                    To = account.UserEmail
                };

                using (MailMessage mm = new MailMessage(model.From, model.To))
                {
                    mm.Subject = model.Subject;
                    mm.Body = BodyRegisterMail(account.UserFullName);
                    mm.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(model.From, model.Password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }

                TempData["message"] = "Đăng ký tài khoản thành công!";
                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                _toastNotification.Error("Đã xảy ra lỗi trong quá trình đăng ký: " + e.Message, 5);
                return View(account);
            }
        }

        [Route("dang-xuat")]
        public IActionResult Logout()
        {
            _toastNotification.Success("Đăng xuất thành công !", 3);
            HttpContext.Session.Remove("customerID");
            return RedirectToAction("Index", "Home");
        }

        public string BodyRegisterMail(string fullName)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Views\\Shared\\Mail", "RegisterSuccessMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{fullName}}", fullName);
            return body;
        }

        public string BodyResetPasswordMail(string pass)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Views\\Shared\\Mail", "ForgotPasswordMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{Password}}", pass);
            return body;
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
        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        private bool UserExists(int id)
        {
            return (_context.Accounts?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        private bool AccountEmailExists(string email)
        {
            return _context.Accounts.Any(e => e.UserEmail == email);
        }
        private bool AccountUserNameExists(string username)
        {
            return _context.Accounts.Any(e => e.UserName == username);
        }
    }
}
