using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IDS.TestData;
using IDSWithEFStoreSeedData;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up..");


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

builder.Services.AddRazorPages();
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.EmitStaticAudienceClaim = true;
})
    .AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
                                                                         opt => opt.MigrationsAssembly(migrationsAssembly)))
    .AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlite(connectionString,
                                                                         opt => opt.MigrationsAssembly(migrationsAssembly)))
    .AddTestUsers(IDS.TestData.TestUsers.Users)
    //.AddInMemoryClients(IDS.TestData.Config.Clients)
    //.AddInMemoryApiResources(IDS.TestData.Config.ApiResources)
    //.AddInMemoryApiScopes(IDS.TestData.Config.ApiScopes)
    //.AddInMemoryIdentityResources(IDS.TestData.Config.IdentityResources);
    ;

//in order to use EntityFramework instead of InMemory stores, install following packages:
//1. Duende.IdentityServer.EntityFramework 
//2. Microsoft.AspNetCore.Identity.EntityFrameworkCore
//3. Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
//4. Microsoft.EntityFrameworkCore.Tools
//5. Microsoft.EntityFrameworkCore.Sqlite (because we will use Sqlite, not SqlServer)

//then run:
//      dotnet ef migrations add InitialIdentityServerMigration -c ConfigurationDbContext
//and then run same for OperationalStore:
//      dotnet ef migrations add InitialIdentityServermigration -c PersistedGrantDbContext

//then run appdate command to apply migrations:
//1. dotnet ef database update -c PersistedGrantDbContext
//2. dotnet ef database update -c ConfigurationDbContext

builder.Services.AddAuthentication();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
        theme: AnsiConsoleTheme.Code)
        .Enrich.FromLogContext();
});

var app = builder.Build();

app.UseIdentityServer();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

//Seed data if "/seed" arg
if(args.Contains("/seed"))
{
    Log.Information("Seeding database...");
    SeedData.EnsureSeedData(app);
    Log.Information("Done seeding database.. Exiting now..");
    return;
}


app.Run();
