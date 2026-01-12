using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Services.TST;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Infrastructure.Validators.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;
using GrpcServer.Infrastructure.Enum;

var builder = WebApplication.CreateBuilder(args);

// Add API controllers
builder.Services.AddControllers();

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

// Register TST Data Seeder as Keyed Service
builder.Services.AddKeyedScoped<TstDataSeeder, TstDataSeeder>(AppCode.TST);

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "User & Group Management API",
        Version = "v1",
        Description = "RESTful CRUD API for Users and Groups with many-to-many relationship management. " +
                     "Supports full CRUD operations on Users and Groups, plus relationship management between them.",
        // Contact = new Microsoft.OpenApi.Models.OpenApiContact
        // {
        //     Name = "API Support",
        //     Email = "support@example.com"
        // }
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

// Seed test data on startup
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredKeyedService<TstDataSeeder>(AppCode.TST);
    await seeder.SeedDataAsync();
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User & Group Management API v1");
        options.DocumentTitle = "User & Group Management API";
    });
}

app.MapControllers();

app.Run();