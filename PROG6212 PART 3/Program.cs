using Microsoft.EntityFrameworkCore;
using PROG6212_PART_3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SQLite Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session Support (REQUIRED for authentication)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Security - prevent JavaScript access
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

// Add HttpContextAccessor for accessing session in controllers
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT: Enable Session Middleware (must be before UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Seed default HR user on first run
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    // Seed default HR user if no users exist
    if (!context.Users.Any())
    {
        var hrUser = new User
        {
            Username = "hradmin",
            PasswordHash = "HR@2025",
            Role = "HR",
            FirstName = "HR",
            LastName = "Administrator",
            Email = "hr@university.edu",
            HourlyRate = 0,
            IsActive = true,
            CreatedDate = DateTime.Now
        };

        context.Users.Add(hrUser);
        context.SaveChanges();

        Console.WriteLine("Default HR user created:");
        Console.WriteLine("Username: hradmin");
        Console.WriteLine("Password: HR@2025");
    }
}

app.Run();