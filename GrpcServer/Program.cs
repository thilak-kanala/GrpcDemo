using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Services.TST;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Infrastructure.Validators.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.GrpcServices.TST;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add API controllers
builder.Services.AddControllers();

// Add gRPC services
builder.Services.AddGrpc();

// Register TST Services as Keyed Services
builder.Services.AddKeyedScoped<IUserService<TstUser>, TstUserService>(AppCode.TST);
builder.Services.AddKeyedScoped<IGroupService<TstGroup>, TstGroupService>(AppCode.TST);
builder.Services.AddKeyedScoped<IUserGroupRelationService<TstUser, TstGroup>, TstUserGroupRelationService>(AppCode.TST);

// Register TST Repositories as Keyed Singletons
builder.Services.AddKeyedSingleton<IUserRepository<TstUser>, TstUserRepository>(AppCode.TST);
builder.Services.AddKeyedSingleton<IGroupRepository<TstGroup>, TstGroupRepository>(AppCode.TST);
builder.Services.AddKeyedSingleton<IUserGroupRelationRepository, TstUserGroupRelationRepository>(AppCode.TST);

// Register TST Validators as Keyed Services
builder.Services.AddKeyedScoped<IValidator<TstUser>, TstUserValidator>(AppCode.TST);
builder.Services.AddKeyedScoped<IValidator<TstGroup>, TstGroupValidator>(AppCode.TST);

// Register TST Mapper as Keyed Singleton
builder.Services.AddKeyedSingleton<TstMapper, TstMapper>(AppCode.TST);

// Register TST Proto Mapper as Keyed Singleton
builder.Services.AddKeyedSingleton<TstProtoMapper, TstProtoMapper>(AppCode.TST);

// Register TST Data Seeder as Keyed Service
builder.Services.AddKeyedScoped<TstDataSeeder, TstDataSeeder>(AppCode.TST);

// Add OpenAPI documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "User & Group Management API";
        document.Info.Version = "v1";
        document.Info.Description = "RESTful CRUD API for Users and Groups with many-to-many relationship management. " +
                     "Supports full CRUD operations on Users and Groups, plus relationship management between them.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Seed test data on startup
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredKeyedService<TstDataSeeder>(AppCode.TST);
        await seeder.SeedDataAsync();
    }
    
    // Configure HTTP request pipeline
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

// Map gRPC services
app.MapGrpcService<TstUserGrpcService>();
app.MapGrpcService<TstGroupGrpcService>();
app.MapGrpcService<TstUserGroupRelationGrpcService>();

app.Run();
