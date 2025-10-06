using RedisBackgroundService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetConnectionString("Redis");
    option.InstanceName = "_Tasks";
});

var host = builder.Build();
host.Run();
