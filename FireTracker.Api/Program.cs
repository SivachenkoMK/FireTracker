using FireTracker.Api.Options;
using FireTracker.Api.Services;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddServiceDefaults();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024;
});
builder.Services.Configure<MessageQueueConfiguration>(builder.Configuration.GetSection(nameof(MessageQueueConfiguration)));
builder.Services.Configure<ImageStorageConfiguration>(builder.Configuration.GetSection(nameof(ImageStorageConfiguration)));
builder.Services.AddSingleton<MessagingService>();
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

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();