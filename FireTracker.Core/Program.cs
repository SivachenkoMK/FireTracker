using FireTracker.Core.Background;
using FireTracker.Core.Options;
using FireTracker.Core.Services;
using FireTracker.Core.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(nameof(RabbitMqConfiguration)));
builder.Services.Configure<AzureServiceBusConfiguration>(builder.Configuration.GetSection(nameof(AzureServiceBusConfiguration)));
builder.Services.Configure<InboundQueueConfiguration>(builder.Configuration.GetSection(nameof(InboundQueueConfiguration)));

_ = isDevelopment ?
    builder.Services.AddSingleton<IMessagingConsumer, RabbitMqMessagingConsumer>() :
    builder.Services.AddSingleton<IMessagingConsumer, AzureServiceBusMessagingConsumer>();

builder.Services.AddSingleton<IncidentService>();

builder.Services.AddHostedService<ConsumerWorker>();

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