using GrpcServer.Infrastructure.Enum;
using GrpcServer.Infrastructure.Repositories.Generic;
using GrpcServer.Infrastructure.Repositories.ABC;
using GrpcServer.Infrastructure.Repositories.INM;
using GrpcServer.Infrastructure.Services.Generic;
using GrpcServer.Infrastructure.Services.ABC;
using GrpcServer.Infrastructure.Services.INM;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc();

// Add API controllers
builder.Services.AddControllers();

// Register INM Repositories with keyed DI
builder.Services.AddKeyedSingleton<IUserRepository, InmUserRepository>(AppCode.INM);
builder.Services.AddKeyedSingleton<IGroupRepository, InmGroupRepository>(AppCode.INM);
builder.Services.AddKeyedSingleton<IUserGroupRelationRepository, InmUserGroupRelationRepository>(AppCode.INM);

// Register INM Services with keyed DI
builder.Services.AddKeyedScoped<IUserService, InmUserService>(AppCode.INM);
builder.Services.AddKeyedScoped<IGroupService, InmGroupService>(AppCode.INM);
builder.Services.AddKeyedScoped<IUserGroupRelationService, InmUserGroupRelationService>(AppCode.INM);

// Register ABC Repositories with keyed DI
builder.Services.AddKeyedSingleton<IUserRepository, AbcUserRepository>(AppCode.ABC);
builder.Services.AddKeyedSingleton<IGroupRepository, AbcGroupRepository>(AppCode.ABC);
builder.Services.AddKeyedSingleton<IUserGroupRelationRepository, AbcUserGroupRelationRepository>(AppCode.ABC);

// Register ABC Services with keyed DI
builder.Services.AddKeyedScoped<IUserService, AbcUserService>(AppCode.ABC);
builder.Services.AddKeyedScoped<IGroupService, AbcGroupService>(AppCode.ABC);
builder.Services.AddKeyedScoped<IUserGroupRelationService, AbcUserGroupRelationService>(AppCode.ABC);

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User & Group Management API",
        Version = "v1",
        Description = "RESTful CRUD API for Users and Groups with many-to-many relationship"
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User & Group Management API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// app.UseHttpsRedirection();
// app.UseAuthorization();

app.MapControllers();

app.Run();