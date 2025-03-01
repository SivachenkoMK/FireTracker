using FireTracker.Analysis;
using FireTracker.Analysis.Options;
using FireTracker.Analysis.Services;
using FireTracker.Analysis.Services.Abstractions;

var builder = Host.CreateApplicationBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

builder.AddServiceDefaults();

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(nameof(RabbitMqConfiguration)));
builder.Services.Configure<AzureServiceBusConfiguration>(builder.Configuration.GetSection(nameof(AzureServiceBusConfiguration)));
builder.Services.Configure<MachineLearningModelConfiguration>(builder.Configuration.GetSection(nameof(MachineLearningModelConfiguration)));
builder.Services.Configure<ImageStorageConfiguration>(builder.Configuration.GetSection(nameof(ImageStorageConfiguration)));

if (isDevelopment)
{
    builder.Services.AddSingleton<IMessagingConsumer, RabbitMqMessagingConsumer>();
    builder.Services.AddSingleton<IMessagingPublisher, RabbitMqMessagingPublisher>();
}
else
{
    builder.Services.AddSingleton<IMessagingConsumer, AzureServiceBusMessagingConsumer>();
    builder.Services.AddSingleton<IMessagingPublisher, AzureServiceBusMessagingPublisher>();
}

builder.Services.AddSingleton<ImageProcessor>();
builder.Services.AddSingleton<InterpretationService>();
builder.Services.AddSingleton<RoutingService>();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Run();