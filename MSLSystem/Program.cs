using MSLSystem.Models;
using MSLSystem.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<UserService>();

builder.Services.AddControllersWithViews();

//builder.Services.AddTransient<UserService>(provider =>
//    new UserService(builder.Configuration.GetConnectionString("OracleDbContext")));

builder.Services.AddScoped<UserService>(provider =>
    new UserService(builder.Configuration.GetConnectionString("OracleDbContext")));

builder.Services.AddScoped<EmailService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // You can adjust the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Essential if using GDPR compliance
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Use HSTS for security in production environments
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
