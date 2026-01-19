# OpenAPI Quick Reference Guide

## Accessing OpenAPI Documentation

### Development Environment

When running the application in Development mode, you have access to:

#### 1. OpenAPI JSON Document
```
http://localhost:5000/openapi/v1.json
```
- Raw OpenAPI 3.0 specification
- Can be imported into tools like Postman, Insomnia, etc.
- Machine-readable format

#### 2. Scalar Interactive UI
```
http://localhost:5000/scalar/v1
```
- Modern, beautiful API documentation interface
- Interactive API testing
- Better than traditional Swagger UI
- Shows request/response examples
- Try out APIs directly in the browser

## Key Features

### ✅ Automatic API Discovery
- All controller endpoints are automatically discovered
- No manual configuration needed for basic endpoints
- XML documentation comments are included automatically

### ✅ Development-Only Exposure
- OpenAPI endpoints only available in Development environment
- Production deployments won't expose documentation
- Security best practice built-in

### ✅ Document Transformers
The project uses document transformers to customize the OpenAPI document:
```csharp
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Your API Title";
        document.Info.Version = "v1";
        document.Info.Description = "API Description";
        return Task.CompletedTask;
    });
});
```

## Customization Options

### Adding Contact Information
```csharp
options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.Contact = new()
    {
        Name = "API Support",
        Email = "support@example.com",
        Url = new Uri("https://example.com/support")
    };
    return Task.CompletedTask;
});
```

### Adding License Information
```csharp
options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.License = new()
    {
        Name = "MIT",
        Url = new Uri("https://opensource.org/licenses/MIT")
    };
    return Task.CompletedTask;
});
```

### Adding Servers
```csharp
options.AddDocumentTransformer((document, _, _) =>
{
    document.Servers.Add(new()
    {
        Url = "https://api.production.com",
        Description = "Production Server"
    });
    return Task.CompletedTask;
});
```

## XML Documentation Comments

The project is configured to generate XML documentation automatically:
- `GenerateDocumentationFile=true` in .csproj
- Comments from your controllers will appear in OpenAPI
- Use standard XML doc comments:

```csharp
/// <summary>
/// Gets a user by ID
/// </summary>
/// <param name="id">The user ID</param>
/// <returns>The user object</returns>
/// <response code="200">Returns the user</response>
/// <response code="404">User not found</response>
[HttpGet("{id}")]
public ActionResult<User> GetUser(int id)
{
    // ...
}
```

## Testing Your API

### Using Scalar UI
1. Start your application
2. Navigate to `http://localhost:5000/scalar/v1`
3. Browse available endpoints
4. Click "Try it out" on any endpoint
5. Fill in parameters
6. Click "Execute"
7. View response

### Using External Tools
1. Export OpenAPI JSON from `http://localhost:5000/openapi/v1.json`
2. Import into:
   - Postman
   - Insomnia
   - Bruno
   - Any OpenAPI-compatible tool

## Migration from Swashbuckle

If you're familiar with Swashbuckle, here are the key differences:

| Swashbuckle | Modern OpenAPI |
|------------|----------------|
| `AddSwaggerGen()` | `AddOpenApi()` |
| `UseSwagger()` | `MapOpenApi()` |
| `UseSwaggerUI()` | `MapScalarApiReference()` |
| `/swagger/v1/swagger.json` | `/openapi/v1.json` |
| `/swagger` | `/scalar/v1` |

## Troubleshooting

### OpenAPI endpoint not accessible
- Ensure you're running in Development mode
- Check `ASPNETCORE_ENVIRONMENT` is set to "Development"
- Verify `app.MapOpenApi()` is called

### Scalar UI not loading
- Ensure `Scalar.AspNetCore` package is installed
- Check `app.MapScalarApiReference()` is called
- Verify the application is running

### XML comments not showing
- Ensure `GenerateDocumentationFile` is `true` in .csproj
- XML comments must be properly formatted
- Rebuild the project after adding comments

## Additional Resources

- [Microsoft OpenAPI Documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview)
- [Scalar GitHub](https://github.com/scalar/scalar)
- [OpenAPI Specification](https://spec.openapis.org/oas/latest.html)
