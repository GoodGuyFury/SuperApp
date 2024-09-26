using MySuparApp.Shared;


var builder = WebApplication.CreateBuilder(args);

RegisteredServices.RegisterServices(builder.Services, builder.Configuration, builder.Logging);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Enable CORS
app.UseCors("AllowSpecificOrigins");

// Use custom authentication middleware
app.UseMiddleware<AuthHandler>();

// Use the global error handler middleware
app.UseMiddleware<GlobalErrorHandler>();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
