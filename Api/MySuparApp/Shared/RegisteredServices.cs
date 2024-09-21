using AdminRepository;
using AppSettingsModel;
using AuthMiddlware;
using AuthTokenRepository;
using Microsoft.Extensions.DependencyInjection;

namespace RegisteredServicesShared
{
    public static class RegisteredServices
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {

            // Register JwtSettings from appsettings.json
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Load GoogleClientId from appsettings.json
            GoogleSettings.GoogleClientId = configuration["GoogleClientId"];

            // Add AuthHandler as a service
            services.AddScoped<AuthHandler>();

            services.AddScoped<AuthToken>();
            services.AddScoped<UserManagement>();

        }
    }
}

