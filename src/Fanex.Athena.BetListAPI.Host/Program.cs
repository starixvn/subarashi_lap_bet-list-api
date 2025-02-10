using Fanex.Athena.BetListAPI.Host.DbSettingConfig;
using Fanex.Athena.BetListAPI.Host.DependencyInjection;
using Fanex.Athena.BetListAPI.Host.HealthChecks;
using Fanex.Athena.BetListAPI.Host.Middlewares;
using Fanex.Athena.BetListAPI.Host.Settings;
using Fanex.Athena.Common.Services.Interfaces;
using Fanex.Athena.Common.Utility;
using Fanex.BetList.Core;
using Fanex.BetList.Core.RemotingRef;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.RemotingRef;
using Fanex.Result.Core.Base;
using Fanex.Result.Core.RemotingRef;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json.Serialization;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.AllowSynchronousIO = true;
});
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
    });

services.AddAuthorization(option =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser();
    defaultPolicy.AuthenticationSchemes.Add(OAuth2IntrospectionDefaults.AuthenticationScheme);
    option.DefaultPolicy = defaultPolicy.Build();
});

services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
services.AddRazorPages();

services.AddLapBetList()
    .AddLapBetListResult(configuration);

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

//Middlewares
app.UsePing();

if (app.Environment.IsDevelopment() || string.Equals(configuration["UseErrorHandler"], "0"))
{
    app.UseDeveloperExceptionPage();
}

app.UseErrorHandler();
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
app.UseDoubleSlashHandler();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(config =>
{
    config.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("This is Bet list API");
    });
    config.MapGet("/test/exception", async context =>
    {
        throw new Exception("Test exception");
    });

    config.MapDefaultControllerRoute()
        .RequireAuthorization();
});

app.Run();
