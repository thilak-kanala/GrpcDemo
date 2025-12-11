using GrpcServer.GrpcServices;
using GrpcServer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services
    .AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapGrpcService<GrpcUserService>();

app.Run();