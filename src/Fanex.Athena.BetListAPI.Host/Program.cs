using IdentityModel.AspNetCore.OAuth2Introspection;
using Fanex.Athena.BetListAPI.Site.Settings;
using Microsoft.AspNetCore.Authorization;
using Aff.YB.DataSync.Service.Host.Middleware;
using Fanex.Athena.BetListAPI.Site.DependencyInjection;
using Fanex.Athena.BetListAPI.Site.Middlewares;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Fanex.Athena.Common.Utility;
using Fanex.RemotingRef;
using Fanex.BetList.Core;
using Fanex.BetList.Core.RemotingRef;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Athena.BetListAPI.Site.DbSettingConfig;
using Newtonsoft.Json.Serialization;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Site.Authentications;
using Fanex.Result.Core.Base;
using Fanex.Result.Core.RemotingRef;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSentry();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
        .AddJsonFile("betlistresultsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile($"betlistresultsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

var services = builder.Services;
var configuration = builder.Configuration;
var identityServer = configuration.GetSection("IdentityServer").Get<IdentityServerSetting>();

//DI
services.AddHealthChecks()
        .AddCheck<WhoAmIHealthCheck>("WhoAmI");

services.AddDistributedMemoryCache();
services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
    .AddOAuth2Introspection(options => 
    {
        options.Authority = identityServer.Host;
        options.ClientId = identityServer.ClientId;
        options.ClientSecret = identityServer.ClientSecret;
        options.CacheDuration = TimeSpan.FromMinutes(identityServer.CacheDuration);
        options.CacheKeyPrefix = "BETLIST_CACHE_TOKEN.";
        options.EnableCaching = true;
        options.SkipTokensWithDots = true;
    })
    .AddBasicAuthentication(options => 
    {
        options.Clients = configuration.GetSection("ServiceClients").Get<List<ServiceClient>>();
    });

services.AddAuthorization(option =>
{

    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser();
    defaultPolicy.AuthenticationSchemes.Add(OAuth2IntrospectionDefaults.AuthenticationScheme);
    defaultPolicy.AuthenticationSchemes.Add("Basic");
    option.DefaultPolicy = defaultPolicy.Build();
});

services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
//services.AddEndpointsApiExplorer();

services.AddLapBetList();

var app = builder.Build();

RefDataTypeContainer
    .ConnectTo(new Uri(configuration.GetSection("AppSettings")["Fanex.RemotingRef.Service.Url"]))
    .FromApplication("Fanex.Athena.BetListAPI.Host");
BetListManager.RegisterRemotingRef(new CacheRef());

DbSettingProviderManager.StartNewSession()
    .WithSqlServer(configuration)
    .UseShardDbSettingProvider(app.Services.GetRequiredService<IShardInfoAccessor>(), configuration.GetSection("StoredProcedurePath").Get<string>())
.Run();

BetListManager.RegisterRemotingRef(new CacheRef());
ResultCoreManager.RegisterRefDataHelper(new RefDataHelper());

//Middleares
app.UsePing();

if (string.Equals(configuration["UseErrorHandler"], "1"))
{
    app.UseErrorHandler();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();

app.UseHealthCheck();
app.UseAuthentication();
app.UseRequestLocalization(options =>
{

    const string EnCultureName = "en-US";
    var enCulture = new CultureInfo(EnCultureName)
    {
        DateTimeFormat = new DateTimeFormatInfo
        {
            ShortDatePattern = "M/d/yyyy",
            ShortTimePattern = "h:mm tt",
            LongDatePattern = "MM/dd/yyyy",
            LongTimePattern = "h:mm:ss tt"
        }
    };
    var csCulture = new CultureInfo("zh-CN");

    options.DefaultRequestCulture = new RequestCulture(EnCultureName);
    options.SupportedCultures = new List<CultureInfo>
                {
                    enCulture
                };
    options.SupportedUICultures = new List<CultureInfo>
                {
                    enCulture,
                    csCulture,
                    new CultureInfo("vi-VN"),
                    new CultureInfo("id-ID"),
                    new CultureInfo("th-TH"),
                };
    options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CustomRequestCultureProvider(context =>
                    {
                        var langCode = context.User.FindFirst("Language")?.Value ?? "en";
                        var culture = Languages.LanguageCodeMappers.GetValueOrDefault(langCode, "en-US");
                        return Task.FromResult(new ProviderCultureResult(culture: EnCultureName, uiCulture: culture));
                    })
                };

});
app.UseRouting();
app.UseServiceTokenValidator();
app.UseAuthorization();
app.UseEndpoints(config =>
{
    config.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("This is Bet list API");
    });
    config.MapDefaultControllerRoute()
        .RequireAuthorization();
});

app.Run();
