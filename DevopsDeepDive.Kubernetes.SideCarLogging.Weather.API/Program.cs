using Elastic.Serilog.Sinks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;
using System.Runtime.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders().AddConsole().AddDebug().AddEventSourceLogger();

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .MinimumLevel.Debug()
//    .CreateLogger();
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(new ElasticsearchJsonFormatter())
            .WriteTo.File($"application-logs/log-.log",
                rollingInterval: RollingInterval.Minute,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}",
                shared: false)
            .CreateLogger();
//Log.Logger = new LoggerConfiguration()
//       .MinimumLevel.Debug()
//       .WriteTo.Console(new ElasticsearchJsonFormatter())
//       .WriteTo.File($"logs/app.log",
//       rollingInterval: RollingInterval.Day,
//       restrictedToMinimumLevel: LogEventLevel.Verbose)
//       .Enrich.FromLogContext()
//       //.Enrich.WithCorrelationId()
//       //.Enrich.WithCorrelationIdHeader()
//       .CreateLogger();
Log.Information($"Log configured for Weather API in Development environment... in {AppDomain.CurrentDomain.BaseDirectory}");
//Log.Logger = new LoggerConfiguration()
//        .MinimumLevel.Information()
//        .WriteTo.Elasticsearch(nodes: [new Uri("http://elasticsearch:9200")]
//            , restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
//        .Enrich.FromLogContext()
//        .CreateLogger();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();


var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Log.Information($"Starting up the Weather API...{Directory.GetCurrentDirectory()}");
app.Run();
