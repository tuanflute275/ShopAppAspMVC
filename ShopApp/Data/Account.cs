using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Account
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? UserPassword { get; set; }

    public string? UserFullName { get; set; }

    public string? UserPhone { get; set; }

    public string? UserAddress { get; set; }

    public string? UserAvatar { get; set; }

    public string? UserEmail { get; set; }

    public int? UserGender { get; set; }

    public int? UserActive { get; set; }

    public int? UserRole { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
