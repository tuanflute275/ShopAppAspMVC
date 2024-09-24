using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public int? CategoryStatus { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
