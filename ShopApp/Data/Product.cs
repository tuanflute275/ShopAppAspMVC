using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public int? ProductCategoryId { get; set; }

    public string? ProductDescription { get; set; }

    public string? ProductImage { get; set; }

    public string? ProductName { get; set; }

    public double? ProductPrice { get; set; }

    public double? ProductSalePrice { get; set; }

    public int? ProductStatus { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Category? ProductCategory { get; set; }
}
