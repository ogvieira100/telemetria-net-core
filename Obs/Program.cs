using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Util;

var builder = WebApplication.CreateBuilder(args);



ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
IWebHostEnvironment environment = builder.Environment;

builder.Configuration.AddJsonFile("appsettings.json", true, true)
                    .SetBasePath(environment.ContentRootPath)
                    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();

var conn =  configuration.GetConnectionString("DefaultConnection");
// Configurar rastreamento
const string serviceName = "roll-dice";

builder.Logging.AddOpenTelemetry(options =>
{
    // Conectar com o rastreamento
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    
    options.AddProcessor(new SqlLogProcessor(conn));
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        ;
        
});

// Adicionar um filtro para registrar apenas logs de nível "Error"
builder.Logging.AddFilter<OpenTelemetryLoggerProvider>("", LogLevel.Trace);

//builder.Logging.AddFilter("MyNamespace.MyClass", LogLevel.Warning); // Exemplo para uma classe específica
//builder.Logging.AddFilter<OpenTelemetryLoggerProvider>("MyNamespace", LogLevel.Error); // Para um namespace


builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddSqlClientInstrumentation()    
          .AddHttpClientInstrumentation()
     // The rest of your setup code goes here
             .AddJaegerExporter(jaegerOptions =>
             {
                 jaegerOptions.Endpoint = new Uri("http://localhost:14268/api/traces");
             })
          .AddConsoleExporter())
          .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter());



var app = builder.Build();

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
