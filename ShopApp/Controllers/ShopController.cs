using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using X.PagedList;

namespace ShopApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        public ShopController(ShopAppAspWebContext context, INotyfService notyfService)
        {
            _context = context;
            _toastNotification = notyfService;
        }
        [Route("san-pham/{slug?}")]
        public async Task<IActionResult> Index(string? slug, string? name, string? sort, int page = 1)
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
            ViewBag.names = name;
            ViewBag.sorts = sort;
            // lisst cate
            var categories = await _context.Categories.Where(x => x.CategoryStatus == 1).ToListAsync();
            ViewBag.Categories = categories;

            var products = await _context.Products.OrderByDescending(x => x.ProductId).Include(p => p.ProductCategory).ToListAsync();
            if (!string.IsNullOrEmpty(slug))
            {
                var cate = await _context.Categories.FirstOrDefaultAsync(x => x.CategorySlug == slug);
                if (cate != null) {
                      products = await _context.Products
                     .Where(x => x.ProductCategory.CategoryId == cate.CategoryId)
                     .OrderByDescending(x => x.ProductId)
                     .ToListAsync();
                }
                else
                {
                    _toastNotification.Success("Không có sản phẩm !", 3);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = await _context.Products
                    .Where(x => x.ProductName.Contains(name))
                    .OrderByDescending(x => x.ProductId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }
            }
            var pagedData = products.ToPagedList(page, limit);
            return View(pagedData);
        }

        [Route("chi-tiet-san-pham/{slug}")]
        public async Task<IActionResult> Details(string slug)
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
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            //var user data
            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null) { 
                ViewBag.customerID = customerID;
            }
            // product detail data
            var productDetail = _context.Products
                .Include(x => x.ProductCategory)
                .Where(x => x.Slug == slug).FirstOrDefault();
            ViewBag.ProductDetail = productDetail;

            // product related
            var productRelated = _context.Products.Where(x =>x.ProductCategoryId != productDetail.ProductCategoryId)
                .OrderByDescending(x => x.ProductId)
                .Take(6);
            ViewBag.ProductRelated = productRelated;
            return View("Details");
        }
    }
}
