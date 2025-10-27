using FlightStorageService.Repository;
using Microsoft.OpenApi.Models;
using FlightStorageService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 
builder.Logging.AddDebug();   

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<FlightCleanupService>();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Flight Storage API",
        Version = "v1",
        Description = "API for searching and storing flight information"
    });
});

builder.Services.AddScoped<FlightRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Storage API v1");
        c.RoutePrefix = "";
    });
}

app.MapControllers();
app.Run();
