using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Models;
using PustokProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(@"Server=DESKTOP-AMTUISF\SQLEXPRESS;Database=PustokProject;Trusted_Connection=true");
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<DataContext>();

builder.Services.AddScoped<LayoutServices>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = "15956077951-h2035st2ttvu09rgntias36ir67ki5u6.apps.googleusercontent.com";
    opt.ClientSecret = "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";
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.HttpContext.Request.Path.Value.StartsWith("/manage"))
        {
            var redirectUri = new Uri(context.RedirectUri);
            context.Response.Redirect("/manage/account/login" + redirectUri.Query);
        }
        else
        {
            var redirectUri = new Uri(context.RedirectUri);
            context.Response.Redirect("/account/login" + redirectUri.Query);
        }

        return Task.CompletedTask;
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
