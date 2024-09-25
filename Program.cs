using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnAccount.Areas.Identity.Data;
using OnAccount.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using OnAccount.Areas.Identity.Services;
using OnAccount.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
//required for Apache reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.RequireHeaderSymmetry = false;
    options.ForwardLimit = 2;
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1")); //reverse proxy, Kestrel defaults to port 5000 which is also set in apsettings.json
    options.KnownProxies.Add(IPAddress.Parse("162.205.232.98")); //server IP public
});
Environment.SetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED", "true");
//configure listen protocals and assert SSL/TLS requirement
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.SslProtocols = SslProtocols.Tls13;
        listenOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;//requires certificate from client
    });
});
//configure connection string from environment variables thus hidding it from production
var environ = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var connectionString = "";
var GC_Email_Pass = "";
if (environ == "Production")
{
    //pulls connection string from environment variables
    connectionString = Environment.GetEnvironmentVariable("MariaDbConnectionStringLocal");
    GC_Email_Pass = Environment.GetEnvironmentVariable(GC_Email_Pass);
}
else
{
    //pulls connection string from development local version of secrets.json
    connectionString = builder.Configuration.GetConnectionString("MariaDbConnectionStringRemote");
    GC_Email_Pass = builder.Configuration.GetConnectionString("GC_Email_Pass");

}
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
Environment.SetEnvironmentVariable("DbConnectionString", connectionString);//this is used in services to access the string
Environment.SetEnvironmentVariable("GC_Email_Pass", GC_Email_Pass);

var serverVersion = new MySqlServerVersion(new Version(10, 6, 11));
//use this option for a stable normal configuration
builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion, options => options.EnableRetryOnFailure())
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

//use for code first migrations with mysql only
/*builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion, options => options.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()

);*/
builder.Services.AddHttpContextAccessor(); // This is required to inject the UserService into cshtml files
builder.Services.AddScoped<UserService>(); //This is a non-singleton class providing the current users information via dependency injection.
builder.Services.AddScoped<DbConnectorService>();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

});
builder.Services.AddAuthorization();

//addition of encryption methods for deployment on linux
builder.Services.AddDataProtection().UseCryptographicAlgorithms(
    new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });
builder.Services.AddMvc();
builder.Services.AddResponseCompression(options =>
 options.MimeTypes = ResponseCompressionDefaults
 .MimeTypes.Concat(new[] { "application/octet-stream:" })
);
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    //app.UseHttpsRedirection(); // Appache webserver handles.  <-- Do not use! This is retained as a reminder.
    //app.UseHsts(); <-- Do not use! This is retained as a reminder.
}
else
{
    //app.UseDeveloperExceptionPage(); // This can be enabled to enable http error reporting. Disable for production!
    //app.UseHttpsRedirection(); // <-- Do not use! This is retained as a reminder. Appache2 is responsible for https.
    //app.UseHsts(); <-- Do not use! This is retained as a reminder.
}

var configuration = builder.Configuration;
var value = configuration.GetValue<string>("value");
app.UseResponseCompression();
app.UseAuthorization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseResponseCompression();
app.UseStaticFiles();
app.UseCookiePolicy();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=FirstLogin}/{id?}");
app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=AdminController}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Roles",
    pattern: "{controller=AdminController}/{action=CreateRole}/{id?}");
app.MapControllerRoute(
    name: "Email",
    pattern: "{controller=AdminController}/{action=Email}/{id?}");
app.MapControllerRoute(
    name: "Lock",
    pattern: "{controller=AdminController}/{action=Lock}/{id?}");
app.MapControllerRoute(
    name: "Unlock",
    pattern: "{controller=AdminController}/{action=Unlock}/{id?}");
app.MapControllerRoute(
    name: "UserAccounts",
    pattern: "{controller=AdminController}/{action=ManageAccounts}/{id?}");
app.MapControllerRoute(
    name: "Accounting",
    pattern: "{controller=AccountingController}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Accounting",
    pattern: "{controller=AccountingController}/{action=ChartOfAccounts}/{id?}");

app.MapRazorPages();
app.Run();
