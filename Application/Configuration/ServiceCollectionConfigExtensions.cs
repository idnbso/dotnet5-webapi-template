using System.Text;
using AutoMapper;
using Domain.Mapping;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Application.Configuration
{
    public static class ServiceCollectionConfigExtensions
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            services.AddSingleton<AutoMapper.IConfigurationProvider>(config);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));
        }


        public static void AddAuthConfiguration(this IServiceCollection services, string tokenKey)
        {
            var key = Encoding.ASCII.GetBytes(tokenKey);

            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate()
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton<IJWTAuthenticationManager>(x => new JWTAuthenticationManager(tokenKey, x.GetService<IHttpContextAccessor>()));
        }

        public static void AddCorsConfiguration(this IServiceCollection services, string[] allowedOrigins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                "CorsPolicy",
                builder => builder.WithOrigins(allowedOrigins)
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials());
            });
        }
    }
}