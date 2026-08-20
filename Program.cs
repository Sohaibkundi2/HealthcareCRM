using HealthcareCRM.Services;
using Microsoft.EntityFrameworkCore;
using HealthcareCRM.Models;
using HealthcareCRM.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<DashboardService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "HealthcareCRM API",
        Version = "v1",
        Description = "REST API for HealthcareCRM — clinic management system."
    });
});

var app = builder.Build();

// Global error handler — must be first
app.UseGlobalErrorHandler();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthcareCRM API v1");
    c.DocumentTitle = "HealthcareCRM API";
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "landing",
    pattern: "",
    defaults: new { controller = "Home", action = "Landing" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();