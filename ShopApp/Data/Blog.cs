using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Blog
{
    public int BlogId { get; set; }

    public string? BlogDescription { get; set; }

    public string? BlogImage { get; set; }

    public string? BlogName { get; set; }
}
