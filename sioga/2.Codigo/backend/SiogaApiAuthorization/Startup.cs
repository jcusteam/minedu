using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Refit;
using SiogaApiAuthorization.Application.Query;
using SiogaApiAuthorization.Clients;
using SiogaApiAuthorization.Handler;
using SiogaApiAuthorization.Proxy;
using SiogaApiAuthorization.Service.Contracts;
using SiogaApiAuthorization.Service.Implementation;

namespace SiogaApiAuthorization
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("MineduPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                });
            });
            services.AddMemoryCache();
            services.AddControllers();

            services.AddMediatR(typeof(FindAccionAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindAllAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindMenuAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindModuloAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindRolAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindSesionAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindUsuarioAuthorizationHandler.Handler).Assembly);
            services.AddMediatR(typeof(FindTokenAuthorizationHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(FindTokenAuthorizationHandler.Handler));

            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddSingleton<IAuthProxy, AuthProxy>();
            services.AddSingleton<IAutorizationService, AutorizationService>();
            services.AddSingleton<IAuthStore, AuthStore>();
            services.AddTransient<RefitHandler>();

            services.AddRefitClient<IUsuarioInstitucionAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:SubvencionUsuarioInstitucionApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddHttpClient("BootService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:BootApi:Url"]);
            });

            services.AddHttpClient("UsuarioDataService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:UsuarioDataApi:Url"]);
            });

            services.AddHttpClient("RolUsuarioDataService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:RolUsuarioDataApi:Url"]);
            });

            services.AddHttpClient("MenuDataService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:MenuDataApi:Url"]);
            });

            services.AddHttpClient("AccionDataService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:AccionDataApi:Url"]);
            });

            services.AddHttpClient("SesionDataService", config =>
            {
                config.BaseAddress = new Uri(Configuration["Apis:SesionDataApi:Url"]);
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authorization API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Authorization API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("MineduPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
