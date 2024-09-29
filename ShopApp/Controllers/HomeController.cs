using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using System.Diagnostics;
using ShopApp.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using DocumentFormat.OpenXml.InkML;

namespace ShopApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopAppAspWebContext _context;

        public HomeController(ILogger<HomeController> logger, ShopAppAspWebContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("")]
        [Route("trang-chu")]
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
                log.UserName = User.Identity.Name;
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

            // data blog
            var blogs =  _context.Blogs.OrderByDescending(x => x.BlogId).Take(3);
            ViewBag.Blogs = blogs;

            // data new product
            var newProducts = _context.Products.OrderByDescending(x => x.ProductId).Take(8);
            ViewBag.NewProducts = newProducts;

            //data sale product
            var saleProducts = _context.Products.Where(x => x.ProductSalePrice > 0).OrderByDescending(x => x.ProductId).Take(8);
            ViewBag.SaleProducts = saleProducts;
            return View();
        }

        [Route("gioi-thieu")]
        public IActionResult About()
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
                log.UserName = User.Identity.Name;
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
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
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
                log.UserName = User.Identity.Name;
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
            return View();
        }

        [Route("bai-viet")]
        public async Task<IActionResult> Blog(int page)
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
                log.UserName = User.Identity.Name;
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

            int limit = 9;
            page = page <= 1 ? 1 : page;
            var blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
            var pagedData = blogs.ToPagedList(page, limit);
            return View("Blog",pagedData);
        }

        [Route("bai-viet/{slug}")]
        public async Task<IActionResult> BlogDetails(string slug)
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
                log.UserName = User.Identity.Name;
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

            var blogDetail = await _context.Blogs.Where(x => x.Slug == slug).FirstOrDefaultAsync();
            ViewBag.BlogDetail = blogDetail;
            var blogRecent = _context.Blogs.Where(x => x.Slug != slug).OrderByDescending(x => x.BlogId).Take(7);
            ViewBag.BlogRecent = blogRecent;
            return View("BlogDetail");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
