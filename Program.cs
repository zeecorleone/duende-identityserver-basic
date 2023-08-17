using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IDS.Database;
using IDS.TestData;
using IDSWithEFStoreSeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection.Metadata;



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/startuplogs.txt")
    .CreateBootstrapLogger();

Log.Information("Starting up..");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    var migrationsAssembly = typeof(Config).Assembly.GetName().Name;

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
    });

    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();


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
        .AddAspNetIdentity<IdentityUser>()
        //.AddTestUsers(IDS.TestData.TestUsers.Users)
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

    //in order to use Aspnet Identity users instead of in-memory install following packages:
    //1. Duende.IdentityServer.AspNetIdentity 
    //2. add: builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
    // then run migrations:
    //      dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext
    // then run update command to apply migrations
    //      dotnet ef database update -c ApplicationDbContext
    //
    // Now remove TestUserStore references from following, and update with actual
    //1. Pages > Account > Login > Login.cshtml.cs
    //      Remove TestUserStore and Inject SignInManager and user that instead
    //1. Pages > ExternalLogin > Callback > Callback.cshtml.cs
    //      Remove TestUserStore and Inject SignInManager and user that instead

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
            .Enrich.FromLogContext()
         .WriteTo.File("logs/log.txt", outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");

    });

    var app = builder.Build();

    app.UseIdentityServer();

    //app.MapGet("/", () => "Hello World!");

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages().RequireAuthorization();

    //Seed data if "/seed" arg
    if (args.Contains("/seed"))
    {
        Log.Information("Seeding database...");
        SeedData.EnsureSeedData(app);
        Log.Information("Done seeding database.. Exiting now..");
        return;
    }


    app.Run();


}
catch(Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
