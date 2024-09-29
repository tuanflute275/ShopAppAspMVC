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
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim != null)
            {
                var userId = userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
                var listCartByUser = await _context.Carts.Include(p => p.Product).Include(a => a.User).Where(x => x.User.UserId == userId).ToListAsync();
                return View(listCartByUser);
            }
            else
            {
                //return RedirectToAction("Login", "User");
                return RedirectToAction("Login", "User", new { returnUrl = !string.IsNullOrEmpty(HttpContext.Request.Path) ? HttpContext.Request.Path.ToString() : "" });
            }
        }

        public async Task<IActionResult> AddToCart(int? accId, int prodId, int quantity = 1)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim != null)
            {
                accId = userIdClaim != null ? Convert.ToInt32(userIdClaim.Value) : 0;
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

        public async Task<IActionResult> Update(int Id, int ProdId, int quantity)
        {
            var prodFoundInCart = await _context.Carts.FirstOrDefaultAsync(x => x.CartId == Id);
            var prodFound = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == ProdId);
            if (prodFoundInCart != null)
            {
                prodFoundInCart.Quantity = quantity;
                prodFoundInCart.TotalAmount += Convert.ToInt32(prodFound?.ProductSalePrice > 0 ? prodFound?.ProductSalePrice * quantity : prodFound?.ProductPrice * quantity);
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
