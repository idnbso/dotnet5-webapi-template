using System.Text;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Domain.Mapping;
using Serilog;
using System;
using Application.Configuration;
using Infrastructure.Data.Access;

namespace aspnet
{
    public class Startup
    {
        private readonly string _tokenKey;
        private readonly string[] _allowedOrigins;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _tokenKey = configuration.GetValue<string>("TokenKey");
            configuration.GetSection("CorsSettings:AllowedOrigins").Bind(_allowedOrigins);

            DataAccessConstants.DefaultConnectionString = configuration.GetConnectionString("DefaultConnection");

            Log.Information($"Starting up in Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}...");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapperConfiguration();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "aspnet", Version = "v1" });
            });

            services.AddHttpContextAccessor();

            services.AddAuthConfiguration(_tokenKey);
            services.AddCorsConfiguration(_allowedOrigins);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "aspnet v1"));
            }
            else
            {
                app.UseExceptionHandler("/errors");
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
