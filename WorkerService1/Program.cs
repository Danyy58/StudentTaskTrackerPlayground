using Microsoft.EntityFrameworkCore;
using UserDeletionBGService;
using WorkerService1;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

