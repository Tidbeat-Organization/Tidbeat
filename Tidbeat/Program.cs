using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Tidbeat;
using Tidbeat.Data;
using Tidbeat.Hub;
using Tidbeat.Middlewares;
using Tidbeat.Models;
using Tidbeat.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
builder.Logging.ClearProviders();

services.AddAuthentication().AddGoogle(
    options =>
    {
        options.ClientId = configuration["Authentication:Google:ClientId"];
        options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
        options.ClaimActions.MapJsonKey("name", "name");
    }
);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var sendGridKey = builder.Configuration["SendGridKey"];

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// N�o necessita de conta confirmada: ALTERAR DEPOIS PARA true
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
services.Configure<MvcViewOptions>(options => {
    options.ViewEngines.Add(new HtmlViewEngine());
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(options => {
    options.SendGridKey = sendGridKey;
});
//builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IChatBeatService, ChatBeatService>();

builder.Services.Configure<IdentityOptions>(opts => {
    opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    opts.Lockout.MaxFailedAccessAttempts = 5;
});

services.AddLocalization(options => options.ResourcesPath = "Resources");
services.AddRazorPages().AddDataAnnotationsLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options => {
    var supportedCultures = new[] {
        new CultureInfo("en-US"),
        new CultureInfo("pt-PT")
    };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedUICultures = supportedCultures;
});

services.AddSignalR().AddAzureSignalR();

var app = builder.Build();
var env = app.Services.GetService<IWebHostEnvironment>();
app.UseRequestLocalization();
/*
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
            Path.Combine(env.ContentRootPath, "Views", "VSdoc")),
    RequestPath = "/VSdoc"
});
*/
app.UseMiddleware<CultureMiddleware>();

// 404 Error Handling
/*
app.Use(async (context, next) => {
    await next();

    if (context.Response.StatusCode == 404) {
        context.Response.Clear();
        context.Response.StatusCode = 404;
        context.Request.Path = "/Home/Error404";
        await next();
    }
});
*/
app.UseStatusCodePagesWithRedirects("/Home/Error404");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapHub<ChatHub>("/chat");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using var scope = app.Services.CreateScope();

await Configurations.CreateStartingRoles(scope.ServiceProvider);
await Configurations.CreateStartingUsers(scope.ServiceProvider);
//await Configurations.CreateStartingPosts(scope.ServiceProvider);

app.Run();
