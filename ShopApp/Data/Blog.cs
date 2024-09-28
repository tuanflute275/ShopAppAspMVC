using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Blog
{
    public int BlogId { get; set; }
    public string? BlogDescription { get; set; }
    public string? BlogImage { get; set; }
    public string? BlogName { get; set; }
    public string? Slug { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public int? UserId { get; set; }
    public virtual Account? User { get; set; }
}
