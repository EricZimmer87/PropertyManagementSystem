using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Authentication;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Handlers;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Seeding;
using PropertyManagement.Api.Services.Bookings;
using PropertyManagement.Api.Services.Email;
using PropertyManagement.Api.Services.Guests;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Exception Handling
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Database connection string
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

// Register the DbContext with the dependency injection container
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    DbSeeding.Configure(options); // Seed database for development testing
});

// Activate Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        // Password
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User
        options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// For Google OAuth
builder.Services.AddAuthentication()
    .AddGoogleOpenIdConnect(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GoogleOpenIdConnect:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:GoogleOpenIdConnect:ClientSecret"];
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "PropertyManagementCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    //options.LoginPath = "/Identity/Account/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;

    // Deny access if user's IsActive is false
    options.EventsType = typeof(CookieValidationEvents);
});

builder.Services.AddScoped<CookieValidationEvents>();

// Add authorization services to dependency injection container
builder.Services.AddAuthorization();

// Add email service
builder.Services.AddTransient<IEmailService, EmailService>();
// Guest Service
builder.Services.AddScoped<IGuestService, GuestService>();
// Booking Service
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// Exception handling
app.UseExceptionHandler();

// Add CORS
app.UseCors("Angular");

// Seed admin user
using var scope = app.Services.CreateScope();
await AdminSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
