using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using ShopApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using NuGet.Protocol;
using ShopApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add AutoMapper Service
builder.Services.AddAutoMapper(typeof(Program));

// Add IToastNotify Service
builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 5000
});
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
    config.HasRippleEffect = true;
});

// Add Razor Options Service
builder.Services.AddControllersWithViews()
    .AddRazorOptions(opts =>
    {
        opts.ViewLocationFormats.Add("/{0}.cshtml");
    });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".tfl.session";
    options.Cookie.HttpOnly = true;
});

// Authentication - Authorize Servicess
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    //options.AccessDeniedPath = new PathString("/Manager/Home/Index");
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = "tfl.cookie",
        Path = "/",
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.SameAsRequest
    };
    options.LoginPath = "/Admin/Logging";
    options.ReturnUrlParameter = "returnUrl";
    options.SlidingExpiration = true;
});

// Configure the connect to MSSQL 
builder.Services.AddDbContext<ShopAppAspWebContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: options => { options.EnableRetryOnFailure(); }));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

// Using cookie and authen -author in this app
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

// Using toast notify and notyf in this app
app.UseNToastNotify();
app.UseNotyf();

app.MapControllers();

// Add Controller Route in area Admin
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();