using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Converters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Config Serilog
builder.Host
    .ConfigureAppConfiguration((hostingcontext, config) => config
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{hostingcontext.HostingEnvironment.EnvironmentName}.json", true, true))
    .UseSerilog((ctx, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.ColoredConsole();
    }).UseDefaultServiceProvider(options => options.ValidateScopes = false); // needed for mediatr DI;

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });


await using var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteResponse});

await app.RunAsync("http://localhost:8080");

static Task WriteResponse(HttpContext context, HealthReport result)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions
    {
        Indented = true
    };

    using var stream = new MemoryStream();
    using (var writer = new Utf8JsonWriter(stream, options))
    {
        writer.WriteStartObject();
        writer.WriteString("status", "success");
        writer.WriteString("message", "OK");
        writer.WriteNull("data");
        writer.WriteEndObject();
    }

    var json = Encoding.UTF8.GetString(stream.ToArray());

    return context.Response.WriteAsync(json);
}
