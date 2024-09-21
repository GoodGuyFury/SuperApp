using Microsoft.EntityFrameworkCore;
using ApplicationDbContextShared;
using AuthMiddlware;
using Serilog;
using RegisteredServicesShared;

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set minimum log level
    .WriteTo.Console() // Log to console
    .WriteTo.File("logs/errorlog.txt", rollingInterval: RollingInterval.Day) // Log file configuration
    .CreateLogger();

Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();  // Remove default providers if needed
builder.Logging.AddSerilog(logger);  // Use Serilog

// Register DbContext with connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


RegisteredServices.RegisterServices(builder.Services, builder.Configuration);

// Add CORS policy using the allowed origins from the configuration
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins(allowedOrigins)
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
