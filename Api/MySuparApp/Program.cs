using WebConfiguration;
using AuthMiddlware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<AuthHandler>(); // Register the middleware

var app = builder.Build();

app.UseCors(builder =>
{
    builder.WithOrigins(WebConfig.AllowedOrigins)
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuthHandler>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
