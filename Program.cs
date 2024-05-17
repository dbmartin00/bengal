using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights.AspNetCore;
using Microsoft.FeatureManagement.Telemetry.ApplicationInsights;
using Microsoft.FeatureManagement;
using extra;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddAzureAppConfiguration(o =>
    {
        o.Connect("Endpoint=https://split-bengal-appconfig.azconfig.io;Id=+42U;Secret=qfPXE0oosBybDpUlrkzAlgfc/uYsq3e56Zd+gg1i36s=");

        o.UseFeatureFlags();
    });

// Add Application Insights telemetry.
builder.Services.AddApplicationInsightsTelemetry(
    new ApplicationInsightsServiceOptions
    {
        ConnectionString = "InstrumentationKey=4b363de2-7e96-41c8-b08a-3e98c8b37a5a;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=924e9d6e-2177-4581-bb85-0d5d78047977",
        EnableAdaptiveSampling = false
    })
    .AddSingleton<ITelemetryInitializer, TargetingTelemetryInitializer>();

builder.Services.AddHttpContextAccessor();

// Add Azure App Configuration and feature management services to the container.
builder.Services.AddAzureAppConfiguration()
    .AddFeatureManagement()
    .WithTargeting<ExampleTargetingContextAccessor>()
    .AddTelemetryPublisher<ApplicationInsightsTelemetryPublisher>();


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Use Azure App Configuration middleware for dynamic configuration refresh.
app.UseAzureAppConfiguration();

// Add TargetingId to HttpContext for telemetry
app.UseMiddleware<TargetingHttpContextMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
