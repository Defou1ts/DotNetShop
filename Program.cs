using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleShop.Data;
using SimpleShop.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));


// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

// MVC + авторизация по умолчанию
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    // Все действия по умолчанию требуют аутентифицированного пользователя
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Caching
builder.Services.AddMemoryCache();

var app = builder.Build();

// Seed DB (sync startup init)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
