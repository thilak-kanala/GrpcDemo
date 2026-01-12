using GrpcServer.Infrastructure.Services.Common;
using GrpcServer.Infrastructure.Services.TST;
using GrpcServer.Infrastructure.Repositories.Common;
using GrpcServer.Infrastructure.Repositories.TST;
using GrpcServer.Infrastructure.Validators.Common;
using GrpcServer.Infrastructure.Validators.TST;
using GrpcServer.Infrastructure.Mappers.TST;
using GrpcServer.Infrastructure.Models.TST;

var builder = WebApplication.CreateBuilder(args);

// Add API controllers
builder.Services.AddControllers();

// Register TST Services
builder.Services.AddScoped<IUserService<TstUser>, TstUserService>();
builder.Services.AddScoped<IGroupService<TstGroup>, TstGroupService>();
builder.Services.AddScoped<IUserGroupRelationService<TstUser, TstGroup>, TstUserGroupRelationService>();

// Register TST Repositories as singletons
builder.Services.AddSingleton<IUserRepository<TstUser>, TstUserRepository>();
builder.Services.AddSingleton<IGroupRepository<TstGroup>, TstGroupRepository>();
builder.Services.AddSingleton<IUserGroupRelationRepository, TstUserGroupRelationRepository>();

// Register TST Validators
builder.Services.AddScoped<IValidator<TstUser>, TstUserValidator>();
builder.Services.AddScoped<IValidator<TstGroup>, TstGroupValidator>();

// Register TST Mapper
builder.Services.AddSingleton<TstMapper>();

// Register TST Data Seeder
builder.Services.AddScoped<TstDataSeeder>();

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
    var seeder = scope.ServiceProvider.GetRequiredService<TstDataSeeder>();
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