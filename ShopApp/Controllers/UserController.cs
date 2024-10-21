using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Data;
using ShopApp.Models.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using ClosedXML.Excel;
using System.Data;
using Order = ShopApp.Data.Order;
using DataTable = System.Data.DataTable;

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
        [Route("thong-tin-ca-nhan")]
        public IActionResult Detail(int id)
        {
            var user = _context.Accounts.Find(id);
            return View(user);
        }

        [HttpPost]
        [Route("thong-tin-ca-nhan")]
        public async Task<IActionResult> Detail(string? oldImage, IFormFile? fileUpload, Account account)
        {
            if (account == null)
            {
                return BadRequest("Account is null");
            }
            if (string.IsNullOrWhiteSpace(account.UserName))
            {
                ModelState.AddModelError("UserName", "Tên tài khoản không được bỏ trống!");
            }
            else if (AccountUserNameExists(account.UserName, account.UserId))
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
            else if (AccountEmailExists(account.UserEmail, account.UserId))
            {
                ModelState.AddModelError("UserEmail", "Email đã tồn tại trong hệ thống!");
            }
            // Nếu ModelState không hợp lệ, trả lại view với dữ liệu lỗi
            if (!ModelState.IsValid)
            {
                return View(account);
            }
            var user = _context.Accounts.Find(account.UserId);
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "accounts", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "accounts", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                user.UserAvatar = fileUpload.FileName;

            }
            else
            {
                user.UserAvatar = oldImage;
            }
            user.UserName = account.UserName;
            user.UserFullName = account.UserFullName;
            user.UserEmail = account.UserEmail;
            user.UserPhone = account.UserPhone;
            user.UserGender = account.UserGender;
            user.UserAddress = account.UserAddress;
            _context.Update(user);
            _toastNotification.Success("Cập nhật thông tin tài khoản thành công!");
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // update session
            HttpContext.Session.SetInt32("customerID", user.UserId);
            HttpContext.Session.SetString("customerName", user.UserName);
            HttpContext.Session.SetString("customerAvatar", user.UserAvatar != null ? user.UserAvatar : "null");
            HttpContext.Session.SetString("customerEmail", user.UserEmail != null ? user.UserEmail : "null");
            return RedirectToAction("Detail", new { id = account.UserId });
        }

        [HttpGet]
        [Route("thay-doi-mat-khau")]
        public IActionResult ChangePass()
        {
            return View();
        }

        [HttpPost]
        [Route("thay-doi-mat-khau")]
        public async Task<IActionResult> ChangePass(ChangePassModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var user = _context.Accounts.Find(customerID);
                if(user != null)
                {
                    if (ModelState.IsValid)
                    {
                        if(user.UserPassword == CreateMD5(model.OldPassword))
                        {
                            if(user.UserPassword == CreateMD5(model.NewPassword))
                            {
                                _toastNotification.Error("Mật khẩu mới không được trùng với mật khẩu hiện tại!");
                            }
                            else
                            {
                                if (model.NewPassword == model.CFPassword)
                                {
                                    user.UserPassword = CreateMD5(model.NewPassword);
                                    _context.Entry(user).State = EntityState.Modified;
                                    await _context.SaveChangesAsync();
                                    _toastNotification.Success("Thay đổi mật khẩu thành công!");
                                    return RedirectToAction(nameof(ChangePass));
                                }
                                else
                                {
                                    ModelState.AddModelError("CFPassword", "Mật khẩu nhập lại không trùng khớp!");
                                }
                            }
                        }
                        else
                        {
                            _toastNotification.Error("Mật khẩu hiện tại không chính xác!");
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
           
            return View(model);
        }

        [HttpGet]
        [Route("theo-doi-don-hang")]
        public async Task<IActionResult> MyOrder(int page = 1)
        {
            int limit = 10;
            var customerID = HttpContext.Session.GetInt32("customerID");
            if(customerID != null)
            {
                var orders = await _context.Orders.Include(o => o.User)
                .Include(x => x.OrderDetails)
                .Where(x => x.User.UserId == customerID)
                .ToListAsync();
                var pagedData = orders.ToPagedList(page, limit);
                return View(pagedData);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("theo-doi-chi-tiet-don-hang")]
        public async Task<IActionResult> DetailOrder(int orderId, int page = 1)
        {
            int limit = 10;
            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var orderFound = _context.Orders.Find(orderId);
                ViewBag.Order = orderFound;
                var orderDetails = await _context.OrderDetails.Include(od => od.Product)
                .Where(x => x.OrderId == orderId).ToListAsync();
                var pagedData = orderDetails.ToPagedList(page, limit);
                return View(pagedData);
            }
            else
            {
                return NotFound();
            }
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
            try
            {
                Log log = new Log();
                log.TimeActionRequest = null;
                log.TimeLogout = null;
                log.TimeLogin = DateTime.Now;
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string workstationName = ipAddress != null ? System.Net.Dns.GetHostEntry(ipAddress).HostName : "Unknown";
                log.WorkTation = workstationName;
                ipAddress = ipAddress.Equals("::1") ? "127.0.0.1" : ipAddress;
                log.IpAdress = ipAddress;
                log.UserName = model.Username;
                string fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                log.Request = fullUrl;
                log.Response = "";
                // save to db log
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
                    HttpContext.Session.SetString("customerName", checkAccount.UserName);
                    HttpContext.Session.SetString("customerAvatar", checkAccount.UserAvatar != null ? checkAccount.UserAvatar : "null");
                    HttpContext.Session.SetString("customerEmail", checkAccount.UserEmail != null ? checkAccount.UserEmail : "null");
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
            try
            {
                Log log = new Log();
                log.TimeActionRequest = null;
                log.TimeLogout = DateTime.Now;
                log.TimeLogin = null;
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                string workstationName = ipAddress != null ? System.Net.Dns.GetHostEntry(ipAddress).HostName : "Unknown";
                log.WorkTation = workstationName;
                ipAddress = ipAddress.Equals("::1") ? "127.0.0.1" : ipAddress;
                log.IpAdress = ipAddress;
                log.UserName = HttpContext.Session.GetString("customerName");
                string fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
                log.Request = fullUrl;
                log.Response = "";
                // save to db log
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            _toastNotification.Success("Đăng xuất thành công !", 3);
            HttpContext.Session.Remove("customerID");
            HttpContext.Session.Remove("customerName");
            HttpContext.Session.Remove("customerAvatar");
            HttpContext.Session.Remove("customerEmail");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("ExportOrder")]
        public async Task<FileResult> ExportOrder(int id)
        {
            if (id != null)
            {
                var orderFound = await _context.Orders.Include(o => o.User)
                               .Include(o => o.OrderDetails)
                               .FirstOrDefaultAsync(x => x.OrderId == id);
                var orderDetails = await _context.OrderDetails.Include(od => od.Product)
                    .Where(x => x.OrderId == orderFound.OrderId).ToListAsync();
                var fileName = "Chi tiết đơn hàng.xlsx";
                return GenerateExcel(fileName, orderFound, orderDetails);
            }
            return null;
        }

        private FileResult GenerateExcel(string fileName, Order order, IEnumerable<OrderDetail> orderDetails)
        {
            // Tạo DataTable cho thông tin đơn hàng
            DataTable orderInfoTable = new DataTable("Thông tin đơn hàng");
            orderInfoTable.Columns.AddRange(new DataColumn[]
            {
        new DataColumn("Chi tiết đơn hàng"),
        new DataColumn("Giá trị")
            });

            // Thêm thông tin đơn hàng vào DataTable
            orderInfoTable.Rows.Add("Chi tiết đơn hàng #" + order.OrderId, "");
            orderInfoTable.Rows.Add("Khách hàng:", order.OrderFullName);
            orderInfoTable.Rows.Add("Địa chỉ:", order.OrderAddress);
            orderInfoTable.Rows.Add("Số điện thoại:", order.OrderPhone);
            orderInfoTable.Rows.Add("Tổng giá trị đơn hàng:", Convert.ToDouble(order.OrderAmount).ToString("N0") + " VND");

            // Tạo DataTable cho danh sách mặt hàng
            DataTable itemsTable = new DataTable("Chi tiết đơn hàng");
            itemsTable.Columns.AddRange(new DataColumn[]
            {
        new DataColumn("Tên sản phẩm"),
        new DataColumn("Giá sản phẩm"),
        new DataColumn("Số lượng"),
        new DataColumn("Tổng tiền")
            });

            // Thêm các mặt hàng vào DataTable
            foreach (var item in orderDetails)
            {
                itemsTable.Rows.Add(item.Product.ProductName, Convert.ToDouble(item.Price).ToString("N0") + " VND", item.Quantity, Convert.ToDouble((item.Price * item.Quantity)).ToString("N0") + " VND");
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                // Thêm worksheet cho thông tin đơn hàng
                var orderInfoWorksheet = wb.Worksheets.Add(orderInfoTable);
                orderInfoWorksheet.Column(1).Width = 30; // Đặt chiều rộng cho cột thông tin
                orderInfoWorksheet.Column(2).Width = 20; // Đặt chiều rộng cho cột giá trị
                orderInfoWorksheet.Rows(1, orderInfoTable.Rows.Count).Style.Font.Bold = true; // Để chữ đậm cho tiêu đề

                // Thêm worksheet cho danh sách mặt hàng
                var itemsWorksheet = wb.Worksheets.Add(itemsTable);
                itemsWorksheet.Column(1).Width = 30; // Đặt chiều rộng cho cột tên sản phẩm
                itemsWorksheet.Column(2).Width = 20; // Đặt chiều rộng cho cột giá sản phẩm
                itemsWorksheet.Column(3).Width = 10; // Đặt chiều rộng cho cột số lượng
                itemsWorksheet.Column(4).Width = 20; // Đặt chiều rộng cho cột tổng tiền
                itemsWorksheet.Rows(1, itemsTable.Rows.Count).Style.Font.Bold = true; // Để chữ đậm cho tiêu đề

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
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

        private bool AccountUserNameExists(string userName, int currentAccountId)
        {
            return _context.Accounts.Any(a => a.UserName == userName && a.UserId != currentAccountId);
        }

        private bool AccountEmailExists(string email, int currentAccountId)
        {
            return _context.Accounts.Any(a => a.UserEmail == email && a.UserId != currentAccountId);
        }
    }
}
