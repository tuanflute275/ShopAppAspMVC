﻿using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using ShopApp.Data;
using ShopApp.Enums;
using ShopApp.Models.ViewModels;
using System.Net.Mail;
using System.Net;
using Order = ShopApp.Data.Order;
using System.Drawing;
namespace ShopApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        public CheckoutController(ShopAppAspWebContext context, INotyfService notyfService)
        {
            _context = context;
            _toastNotification = notyfService;
        }

        [Route("thanh-toan")]
        public async Task<IActionResult> Index()
        {
            try
            {
                Log log = new Log();
                log.TimeActionRequest = DateTime.Now;
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

            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var userData = await _context.Accounts.Where(x => x.UserId == customerID).FirstOrDefaultAsync();
                ViewBag.UserData = userData;
                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(a => a.User).Where(x => x.User.UserId == customerID).ToListAsync();
                ViewBag.ListCartByUser = listCartByUser;

                decimal? subTotal = 0;
                foreach (var item in listCartByUser)
                {
                    subTotal += item.TotalAmount;
                }
                ViewBag.subTotal = subTotal;

                decimal? subQuantity = 0;
                foreach (var item in listCartByUser)
                {
                    subQuantity += item.Quantity;
                }
                ViewBag.subQuantity = subQuantity;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }
        [Route("thanh-toan-thanh-cong")]
        public IActionResult OrderSuccess()
        {
            try
            {
                Log log = new Log();
                log.TimeActionRequest = DateTime.Now;
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
            return View("OrderSuccess");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var listCartByUser = await _context.Carts.Include(p => p.Product)
                    .Include(c => c.Product.ProductCategory).Include(a => a.User)
                    .Where(x => x.User.UserId == customerID).ToListAsync();
                if (listCartByUser.Count() == 0)
                {
                    _toastNotification.Warning("Bạn cần thêm sản phẩm vào giỏ hàng trước khi thanh toán !", 3);
                    return RedirectToAction("Index", "Product");
                }

                if (ModelState.IsValid)
                {
                    // add data to db order
                    order.UserId = customerID;
                    order.OrderDate = DateTime.Now;
                    order.OrderStatus = OrderStatus.CHOXACNHAN;
                    _context.Orders.AddAsync(order);
                    _context.SaveChanges();

                    foreach (var item in listCartByUser)
                    {
                        var details = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalMoney = item.TotalAmount
                        };
                        _context.OrderDetails.AddAsync(details);
                    }

                    EmailModel model = new EmailModel()
                    {
                        Subject = "Xác nhận đơn hàng",
                        To = order.OrderEmail
                    };

                    using (MailMessage mm = new MailMessage(model.From, model.To))
                    {
                        mm.Subject = model.Subject;
                        mm.Body = BodyOrderSuccessMail(order);
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

                    _context.Carts.RemoveRange(listCartByUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(OrderSuccess));
                }

                _toastNotification.Error("Có lỗi xảy ra khi đặt hàng", 3);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }


        public string BodyOrderSuccessMail(Order order)
        {
            string body = string.Empty;
            string unit = "";

            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Views\\Shared\\Mail", "OrderSuccessMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }

            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var listCartByUser = _context.Carts.Include(p => p.Product)
                    .Include(c => c.Product.ProductCategory).Include(a => a.User)
                    .Where(x => x.User.UserId == customerID).ToList();
                foreach (var item in listCartByUser)
                {
                    unit += "<tr>";
                    unit += "<td width = '75 % ' align='left' style='font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px; '>";
                    unit += item.Product.ProductName + " (" + item.Quantity + ") </td>";
                    unit += "<td width = '25 % ' align='left' style='font-family: Open Sans, Helvetica, Arial, sans-serif; font-size: 16px; font-weight: 400; line-height: 24px; padding: 15px 10px 5px 10px; '>";
                    unit += string.Format("{0:#,0} VND", item.Product.ProductSalePrice > 0 ? item.Product.ProductSalePrice : item.Product.ProductPrice) + "</td>";
                    unit += "</tr>";
                }
            }
           
            body = body.Replace("{{CreatedAt}}", order.OrderDate.ToString());
            body = body.Replace("{{FullName}}", order.OrderFullName);
            body = body.Replace("{{Phone}}", order.OrderPhone);
            body = body.Replace("{{Address}}", order.OrderAddress);
            body = body.Replace("{{OrderId}}", order.OrderId.ToString());
            body = body.Replace("{{Unit}}", unit);
            body = body.Replace("{{ToTalPrice}}", string.Format("{0:#,0} VND", order.OrderAmount));
            return body;
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        private static Byte[] ConvertImageToByte(Image img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
