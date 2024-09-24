using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderAddress { get; set; }

    public double? OrderAmount { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? OrderEmail { get; set; }

    public string? OrderFullName { get; set; }

    public string? OrderNote { get; set; }

    public string? OrderPaymentMethods { get; set; }

    public string? OrderPhone { get; set; }

    public int? OrderStatus { get; set; }

    public string? OrderStatusPayment { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Account? User { get; set; }
}
