using System;
using System.Collections.Generic;

namespace ShopApp.Data;

public partial class Log
{
    public int LogId { get; set; }

    public string? UserName { get; set; }

    public string? WorkTation { get; set; }

    public string? Request { get; set; }

    public string? Response { get; set; }

    public string? IpAdress { get; set; }

    public DateTime? TimeLogin { get; set; }

    public DateTime? TimeLogout { get; set; }

    public DateTime? TimeActionRequest { get; set; }
}
