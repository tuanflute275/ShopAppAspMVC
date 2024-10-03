using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using System.Data;
using X.PagedList;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ShopApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public OrderController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? name,string? sort, int page = 1)
        {
            page = page <= 1 ? 1 : page;
            int limit = 10;
            var orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderByDescending(x => x.OrderId).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).Where(x=>x.OrderFullName.Contains(name)).OrderBy(x => x.OrderId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderBy(x => x.OrderId).ToListAsync();
                        break;
                    case "Id-DESC":
                        orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).ToListAsync();
                        break;

                    case "Date-ASC":
                        orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderBy(x => x.OrderId).ToListAsync();
                        break;
                    case "Date-DESC":
                        orders = await _context.Orders.Include(o => o.User).Include(o => o.OrderDetails).OrderByDescending(x => x.OrderId).ToListAsync();
                        break;
                }

            }
            var pagedData = orders.ToPagedList(page, limit);
            return View(pagedData);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderFound = await _context.Orders.Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(x => x.OrderId == id);

            var orderDetails = await _context.OrderDetails.Include(od => od.Product).Where(x => x.OrderId == orderFound.OrderId).ToListAsync();
            ViewBag.orderDetails = null;
            if (orderDetails != null)
            {
                ViewBag.orderDetails = orderDetails;
            }
            return View(orderFound);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? OrderStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                order.OrderStatus = OrderStatus;
                _context.Update(order);
                _context.Entry(order).State = EntityState.Modified;
                _toastNotification.Success("Cập nhật trạng thái đơn hàng thành công !", 3);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Export")]
        public async Task<FileResult> Export(int? id)
        {
            if (id != null)
            {
                var orderFound = await _context.Orders.Include(o => o.User)
                               .Include(o => o.OrderDetails)
                               .FirstOrDefaultAsync(x => x.OrderId == id);
                var orderDetails = await _context.OrderDetails.Include(od => od.Product).Where(x => x.OrderId == orderFound.OrderId).ToListAsync();
                var fileName = "Chi tiết đơn hàng.xlsx";
                return GenerateExcel(fileName, orderFound, orderDetails);
            }
            return null;
        }

        private string ResizeImage(string imagePath, int width, int height)
        {
            var resizedImagePath = Path.ChangeExtension(imagePath, ".resized.png");

            using (var image = Image.Load(imagePath))
            {
                image.Mutate(x => x.Resize(width, height)); 
                image.Save(resizedImagePath); 
            }

            return resizedImagePath;
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _toastNotification.Success("Xóa đơn hàng thành công !", 3);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
