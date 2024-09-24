using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ShopApp.Data;
using System.Net;
using ShopApp.Utils;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ShopApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Route("Admin")]
    public class HomeController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        public HomeController(ShopAppAspWebContext context, INotyfService toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // lấy ra số lượng của từng bảng
            GetRemoteHostIpAddress(HttpContext);
            var userCount = _context.Accounts.Count();
            var onlineUser = UserIpTracker.GetUserCount();
            var orderCount = _context.Orders.Count();
            var categoryCount = _context.Categories.Count();
            var productCount = _context.Products.Count();
            var blogCount = _context.Blogs.Count();
            // truyền dữ liệu về view với ViewBag
            ViewBag.userCount = userCount;
            ViewBag.onlineUser = onlineUser;
            ViewBag.orderCount = orderCount;
            ViewBag.categoryCount = categoryCount;
            ViewBag.productCount = productCount;
            ViewBag.blogCount = blogCount;

            // lấy ra danh sách nhật ký hoạt động người dùng khi đăng nhập vào
            var logs = await _context.Logs.Where(x => x.TimeLogin != null || x.TimeLogout != null || x.TimeActionRequest == null).ToListAsync();
            ViewBag.Logs = logs;

            // Lấy danh sách đơn hàng với OrderStatus = 5 = đã giao hàng thành công
            var orders = await _context.Orders.Where(x => x.OrderStatus == 5)
                //.Include(o => o.OrderDetails)
                //.ThenInclude(od => od.Product)
                .ToListAsync();
            // Chuyển đổi danh sách đơn hàng thành chuỗi JSON
            var ordersJson = JsonConvert.SerializeObject(orders);
            // Lưu trữ chuỗi JSON vào Session
            HttpContext.Session.SetString("globalOrders", ordersJson);
            return View();
        }

        public void GetRemoteHostIpAddress(HttpContext context)
        {
            IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
            string ipv4 = "";
            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }

                ipv4 = remoteIpAddress.ToString();

                // Theo dõi địa chỉ IP của người dùng
                UserIpTracker.TrackUserIp(ipv4);
            }
        }


        [HttpGet]
        [Route("Export")]
        public async Task<FileResult> Export()
        {
            var logs = await _context.Logs.Where(x => x.TimeLogin != null || x.TimeLogout != null || x.TimeActionRequest == null).ToListAsync();
            var fileName = "Nhật ký hoạt động.xlsx";
            return GenerateExcel(fileName, logs);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Log> logs)
        {
            DataTable dataTable = new DataTable("Logs");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Người dùng"),
                new DataColumn("Tên thiết bị"),
                new DataColumn("Địa chỉ IP"),
                new DataColumn("Thời gian đăng nhập"),
                new DataColumn("Thời gian đăng xuất"),
            });

            foreach (var log in logs)
            {
                dataTable.Rows.Add(log.UserName, log.WorkTation, log.IpAdress, log.TimeLogin, log.TimeLogout);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add(dataTable);
                worksheet.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }
    }
}
