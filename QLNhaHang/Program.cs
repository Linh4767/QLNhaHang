using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using QLNhaHang.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QLNhaHangContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("conn")));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=TrangChu}/{id?}");

app.Run();
