using MySuparApp.Repository.Authentication;
using MySuparApp.Models.Shared;
using MySuparApp.Repository.Admin;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MySuparApp.Models.Authentication;
using MySuperApp.Repository.Authentication;

namespace MySuparApp.Shared
{
    public static class RegisteredServices
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
        {
            // Configure Serilog
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set minimum log level
                .WriteTo.Console() // Log to console
                .WriteTo.File("logs/errorlog.txt", rollingInterval: RollingInterval.Day) // Log file configuration
                .CreateLogger();

            Log.Logger = logger;

            logging.ClearProviders();  // Remove default providers
            logging.AddSerilog(logger); // Add Serilog

            // Register DbContext with connection string
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
                    sqlOptions.CommandTimeout(180)).EnableSensitiveDataLogging());

            // Register JwtSettings from appsettings.json
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Load GoogleClientId from appsettings.json
            GoogleSettings.GoogleClientId = configuration["GoogleClientId"] ?? throw new ArgumentNullException("GoogleClientId cannot be null."); ;

            // Register services for dependency injection
            services.AddScoped<AuthHandler>();
            services.AddScoped<IAuthToken, AuthToken>();
            services.AddScoped<IUserManagement, UserManagement>();
            services.AddScoped<IGetUserData, GetUserData>();
            services.AddScoped<IGoogleTokenVerifier, GoogleTokenVerifier>();
            services.AddScoped<ICookieHandler, CookieHandler>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordManager, PasswordManager>();
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Configure CORS policy
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
                {
                    policyBuilder.WithOrigins(allowedOrigins ?? throw new ArgumentNullException("Please add allowed origins."))
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials();
                });
            });

            // Additional services
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
