using HNGTask1.Essential_Enums;

namespace HNGTask1.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(UserEnum.admin.ToString())
               );
                options.AddPolicy("AnalystOnly", policy =>
                policy.RequireRole(UserEnum.analyst.ToString())
               );
            });
            return services;
        } 
    }
}
