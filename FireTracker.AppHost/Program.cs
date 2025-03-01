using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var useServiceBus = false;

if (!useServiceBus)
{
    var username = builder.AddParameter("username", secret: true);
    var password = builder.AddParameter("password", secret: true);

    var rabbitmq = builder.AddRabbitMQ("messaging", username, password, port: 5672)
        .WithManagementPlugin();

    builder.AddProject<FireTracker_Analysis>("analysis")
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);

    builder.AddProject<FireTracker_Api>("api")
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);

    builder.AddProject<FireTracker_Core>("core")
        .WithReference(rabbitmq)
        .WaitFor(rabbitmq);
}
else
{
    builder.AddProject<FireTracker_Analysis>("analysis");
    builder.AddProject<FireTracker_Api>("api");
    builder.AddProject<FireTracker_Core>("core");
}


builder.Build().Run();