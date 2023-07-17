using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.EmitStaticAudienceClaim = true;
})
    .AddTestUsers(IDS.TestData.TestUsers.Users)
    .AddInMemoryClients(IDS.TestData.Config.Clients)
    .AddInMemoryApiResources(IDS.TestData.Config.ApiResources)
    .AddInMemoryApiScopes(IDS.TestData.Config.ApiScopes)
    .AddInMemoryIdentityResources(IDS.TestData.Config.IdentityResources);


var app = builder.Build();

app.UseIdentityServer();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

app.Run();
