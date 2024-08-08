using Cinema.Core.Entites;
using Cinema.Data;
using Cinema.Service.Implementations;
using Cinema.Service.Interfaces;
using Cinema.UI.Filters;
using Cinema.UI.Middlewares;
using Cinema.UI.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddSession();


//builder.Services.AddScoped<SignInManager<AppUser>>();
builder.Services.AddScoped<AuthFilter>();
//builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ICrudService, CrudService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine($"Environment: {environment}");


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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMiddleware<HttpExceptionMiddleware>();
app.UseMiddleware<PasswordResetMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
