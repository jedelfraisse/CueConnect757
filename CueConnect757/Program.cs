using System.Net.Http.Headers;
using ConnectAPA.GraphQL;
using ConnectAPA.Public;
using CueConnect757.Components;
using CueConnect757.DataSQL;
using CueConnect757.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add Microsoft Entra ID authentication
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Add controller support for Microsoft.Identity.Web.UI (for SignIn/SignOut endpoints)
builder.Services.AddControllersWithViews()
	.AddMicrosoftIdentityUI();

builder.Services.AddHttpClient<ApaGraphQlClient>(client =>
{
	client.BaseAddress = new Uri("https://gql.poolplayers.com/graphql");

	var accessToken = builder.Configuration["Apa:AccessToken"];
	if (!string.IsNullOrWhiteSpace(accessToken))
	{
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
	}
});

builder.Services.AddHttpClient<PublicApaClient>(client =>
{
	client.BaseAddress = new Uri("https://gql.poolplayers.com/graphql");
});

builder.Services.AddDbContext<CueConnectDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserInfoService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<ApaSyncService>();
builder.Services.AddHttpClient<ApaLinkingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore/hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
