using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;

namespace ShopApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly INotyfService _toastNotification;
        public CartController(ShopAppAspWebContext context, INotyfService notyfService)
        {
            _context = context;
            _toastNotification = notyfService;
        }

        [Route("gio-hang")]
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

            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(a => a.User).Where(x => x.User.UserId == customerID).ToListAsync();
                return View(listCartByUser);
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }

        [Route("them-gio-hang")]
        public async Task<IActionResult> AddToCart(int? accId, int prodId, int quantity = 1)
        {
            var customerID = HttpContext.Session.GetInt32("customerID");
            if (customerID != null)
            {
                accId = customerID;
                var prodFound = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == prodId);
                var prodFoundInCart = await _context.Carts.FirstOrDefaultAsync(x => x.ProductId == prodId && x.User.UserId == accId);
                if (prodFoundInCart != null)
                {
                    prodFoundInCart.Quantity += quantity != null ? quantity : 1;
                    prodFoundInCart.TotalAmount += Convert.ToInt32(prodFound?.ProductSalePrice > 0 ? prodFound?.ProductSalePrice * quantity : prodFound?.ProductPrice * quantity);
                    _context.Carts.Update(prodFoundInCart);
                }
                else
                {
                    var cart = new Cart()
                    {
                        Quantity = quantity != null ? quantity : 1,
                        TotalAmount = Convert.ToInt32(prodFound?.ProductSalePrice > 0 ? prodFound?.ProductSalePrice * quantity : prodFound?.ProductPrice * quantity),
                        ProductId = prodId,
                        UserId = accId,
                    };
                    _context.Carts.Add(cart);
                }

                await _context.SaveChangesAsync();
                _toastNotification.Success("Thêm giỏ hàng thành công !", 3);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }

        }

        [HttpPost]
        [Route("cap-nhat-gio-hang")]
        public async Task<IActionResult> UpdateCart(int cartId, int productId, string mode)
        {
            var prodFoundInCart = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == cartId);
           var prodFound = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (prodFoundInCart != null)
            {
                if (mode.Equals("plus"))
                {
                    prodFoundInCart.Quantity = prodFoundInCart.Quantity + 1;
                }
                else
                {
                    prodFoundInCart.Quantity = prodFoundInCart.Quantity - 1;
                    if (prodFoundInCart.Quantity <= 0)
                    {
                        _toastNotification.Error("Tối thiểu 1 sản phẩm!", 3);
                        return RedirectToAction(nameof(Index));
                    }
                }
                
                prodFoundInCart.TotalAmount = Convert.ToInt32(prodFound?.ProductSalePrice > 0 ? prodFound?.ProductSalePrice * prodFoundInCart.Quantity : prodFound?.ProductPrice * prodFoundInCart.Quantity);
                _context.Carts.Update(prodFoundInCart);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Cập nhật số lượng sản phẩm thành công !", 3);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }
            var cartFound = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == id);
            if (cartFound != null)
            {
                _context.Remove(cartFound);
                await _context.SaveChangesAsync();
                _toastNotification.Success("Xóa sản phẩm thành công !", 3);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
