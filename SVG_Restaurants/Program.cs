using Microsoft.EntityFrameworkCore;
using SVG_Restaurants.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();

// Add run time compilation to show changes to Razor views without having to restart the application.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); 

// Add Entity Framework Core service registration for SGVRestaurantsContext
builder.Services.AddDbContext<SGVContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SGV"));
});


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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
