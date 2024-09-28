using Microsoft.AspNetCore.Mvc;
using ShopApp.Models;
using System.Diagnostics;
using ShopApp.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("bai-viet")]
        public async Task<IActionResult> Blog(int page)
        {
            int limit = 9;
            page = page <= 1 ? 1 : page;
            var blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
            var pagedData = blogs.ToPagedList(page, limit);
            return View("Blog",pagedData);
        }

        [Route("bai-viet/{slug}")]
        public async Task<IActionResult> BlogDetails(string slug)
        {
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
