using FireTracker.Analysis;
using FireTracker.Analysis.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.Configure<MessageQueueConfiguration>(builder.Configuration.GetSection(nameof(MessageQueueConfiguration)));
builder.Services.Configure<MachineLearningModelConfiguration>(builder.Configuration.GetSection(nameof(MachineLearningModelConfiguration)));
builder.Services.Configure<ImageStorageConfiguration>(builder.Configuration.GetSection(nameof(ImageStorageConfiguration)));

builder.Services.AddSingleton<MessageConsumer>();
builder.Services.AddSingleton<ImageProcessor>();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Run();