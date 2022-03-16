using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services;
using AtlasTracker.Services.Factories;
using AtlasTracker.Services.Interfaces;
using Instructor_AtlasTracker.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BTUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<BTUserClaimsPrincipalFactory>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


// Custom Services
builder.Services.AddScoped<IBTCompanyInfoService, BTCompanyInfoService>();
builder.Services.AddScoped<IBTProjectService, BTProjectService>();
builder.Services.AddScoped<IBTTicketService, BTTicketService>();
builder.Services.AddScoped<IBTLookupService, BTLookupService>();
builder.Services.AddScoped<IBTFileService, BTFileService>();
builder.Services.AddScoped<IBTRolesService, BTRolesService>();
builder.Services.AddScoped<IBTNotificationService, BTNotificationService>();
builder.Services.AddScoped<IBTTicketHistoryService, BTTicketHistoryService>();
builder.Services.AddScoped<IPostgresDateService, PostgresDateService>();
builder.Services.AddScoped<IBTInviteService, BTInviteService>();

builder.Services.AddScoped<IEmailSender, BTEmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


builder.Services.AddMvc();

var app = builder.Build();

var scope = app.Services.CreateScope();
await DataUtility.ManageDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();