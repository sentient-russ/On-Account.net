/*No use or licensing of any kind is authorized with this software. By receiving it, you agree that it will not be used without the express written consent of each of its contributors. This notification supersedes any past agreement, whether written or implied.*/
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using oa.Areas.Identity.Data;
using oa.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using oa.Areas.Identity.Services;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
Environment.SetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED", "true");
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

//required for Apache reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.RequireHeaderSymmetry = false;
    options.ForwardLimit = 2;
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1")); //reverse proxy, Kestrel defaults to port 5000 which is also set in apsettings.json
    options.KnownProxies.Add(IPAddress.Parse("162.205.232.101")); //server IP public
});
//configure listen protocals and assert SSL/TLS requirement
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    serverOptions.ConfigureHttpsDefaults(listenOptions =>
    {
        listenOptions.SslProtocols = SslProtocols.Tls13;
        listenOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;//requires certificate from client
    });
});

var connectionString = "";
var emailPass = "";
var serverVersion = new MySqlServerVersion(new Version(8, 8, 39));
if (builder.Configuration["ASPNETCORE_ENVIRONMENT"] == "Production")
{
    connectionString = Environment.GetEnvironmentVariable("OA_Local");
    emailPass = Environment.GetEnvironmentVariable("GC_Email_Pass");
    if (connectionString == "")
    {
        throw new Exception("ProgramCS: The connection string was null!");
    }

    //db context that auto retries but does not allow migrations with mysql
    builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion, options => options.EnableRetryOnFailure())
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
    );
}
else
{
    //pulls connection string from development local version of secrets.json
    connectionString = builder.Configuration.GetConnectionString("OA_Remote");
    emailPass = builder.Configuration["GC_Email_Pass"];

    //db context which allows migrations but does not auto retry with mysql
    builder.Services.AddDbContext<ApplicationDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion, options => options.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore))
        .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
    );
}
Environment.SetEnvironmentVariable("DbConnectionString", connectionString);//this is used in services to access the string
Environment.SetEnvironmentVariable("GC_Email_Pass", emailPass);//this is used in services to access the string

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

builder.Services.AddScoped<UserService>(); //This is a non-singleton class providing the current users information via dependency injection.
builder.Services.AddSingleton<DbConnectorService>(); //Cannot be a singleton because it will miss the conn str
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor(); // This is required to inject the UserService into cshtml files
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.Services.AddResponseCompression(options =>
 options.MimeTypes = ResponseCompressionDefaults
 .MimeTypes.Concat(new[] { "application/octet-stream:" })
);
builder.Services.AddMvc();
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CurrencyModelBinderProvider());
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
    //app.UseDeveloperExceptionPage(); // This can be enabled to enable http error reporting. Disable for production!
    //app.UseHttpsRedirection(); // Appache webserver handles.  <-- Do not use! This is retained as a reminder.
    //app.UseHsts(); <-- Do not use! This is retained as a reminder.
}
else
{
    app.UseDeveloperExceptionPage(); // This can be enabled to enable http error reporting. Disable for production!
    //app.UseHttpsRedirection(); // <-- Do not use! This is retained as a reminder. Appache2 is responsible for https.
    //app.UseHsts(); <-- Do not use! This is retained as a reminder.
}

//app.UseResponseCompression();
app.UseCookiePolicy();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


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
    name: "Admin",
    pattern: "{controller=AdminController}/{action=ResetTransactions}/{id?}");
app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=AdminController}/{action=ResetAllData}/{id?}");
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
    name: "ChartofAccounts",
    pattern: "{controller=AccountingController}/{action=ChartOfAccounts}/{id?}");
app.MapControllerRoute(
    name: "AddAccount",
    pattern: "{controller=AccountingController}/{action=AddAccounts}/{id?}");
app.MapControllerRoute(
    name: "SaveNewAccountDetails",
    pattern: "{controller=AccountingController}/{action=SaveNewAccountDetails}/{id?}");
app.MapControllerRoute(
    name: "EditAccount",
    pattern: "{controller=AccountingController}/{action=EditAccount}/{id?}");
app.MapControllerRoute(
    name: "ViewAccountDetails",
    pattern: "{controller=AccountingController}/{action=ViewAccountDetails}/{id?}");
app.MapControllerRoute(
    name: "ViewJournalDetails",
    pattern: "{controller=AccountingController}/{action=ViewJournalDetails}/{id?}");
app.MapControllerRoute(
    name: "EmailAdmin",
    pattern: "{controller=AccountingController}/{action=EmailAdmin}/{id?}");
app.MapControllerRoute(
    name: "GetGeneralJournalPage",
    pattern: "{controller=AccountingController}/{action=GetGeneralJournalPage}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AccountingController}/{action=GeneralJournal}/{id?}/{status?}/{JID?}");

app.MapRazorPages();
app.Run();
