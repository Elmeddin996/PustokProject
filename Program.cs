using Microsoft.EntityFrameworkCore;
using PustokProject.DAL.DataAccess;
using PustokProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(@"Server=DESKTOP-AMTUISF\SQLEXPRESS;Database=PustokProject;Trusted_Connection=true");
});

builder.Services.AddScoped<LayoutServices>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
