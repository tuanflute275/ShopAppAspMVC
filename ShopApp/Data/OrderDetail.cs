using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public double? TotalMoney { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
