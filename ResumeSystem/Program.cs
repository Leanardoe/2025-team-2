using OpenAI;
using Microsoft.EntityFrameworkCore;
using ResumeSystem.Models.Database;
using Microsoft.AspNetCore.Identity;
using ResumeSystem.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Enable session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add OpenAI DI
builder.Services.AddSingleton<OpenAIClient>(_ =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

builder.Services.Configure<DefaultAdminSettings>(
	builder.Configuration.GetSection("DefaultAdmin"));

// Add EF Core DI
builder.Services.AddDbContext<ResumeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ResumeContext")));

builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<ResumeContext>()
	.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/SignIn";
	options.ExpireTimeSpan = TimeSpan.FromDays(14);
	options.SlidingExpiration = true;

	options.Events.OnSigningIn = context =>
	{
		if (context.Properties.IsPersistent)
		{
			context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
		}
		return Task.CompletedTask;
	};
});
var app = builder.Build();

// Migrate database automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ResumeContext>();
    db.Database.Migrate(); // Ensures DB is created & schema is up-to-date
}

app.UseStaticFiles();

//create default user
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	await IdentitySeeder.SeedDefaultUserAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
