using IceLandTeria.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Entity Framework Core.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply EF Core migrations automatically when the application starts.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "admin-products",
    pattern: "Admin/Products/{action=Index}/{id?}",
    defaults: new { controller = "Products" });

app.MapControllerRoute(
    name: "admin-categories",
    pattern: "Admin/Categories/{action=Index}/{id?}",
    defaults: new { controller = "Categories" });

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin",
    defaults: new
    {
        controller = "Products",
        action = "Index"
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();