using FireTracker.Api.Options;
using FireTracker.Api.Services;
using FireTracker.Api.Services.Abstractions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
var isServiceBusDisabled = builder.Environment.IsDevelopment();

// Add services to the container.

builder.AddServiceDefaults();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024;
});
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(nameof(RabbitMqConfiguration)));
builder.Services.Configure<AzureServiceBusConfiguration>(builder.Configuration.GetSection(nameof(AzureServiceBusConfiguration)));
builder.Services.Configure<ImageStorageConfiguration>(builder.Configuration.GetSection(nameof(ImageStorageConfiguration)));

_ = isServiceBusDisabled
    ? builder.Services.AddSingleton<IMessagingService, RabbitMqMessagingService>()
    : builder.Services.AddSingleton<IMessagingService, AzureServiceBusMessagingService>();

builder.Services.AddSingleton<StorageService>();
builder.Services.AddScoped<RoutingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();