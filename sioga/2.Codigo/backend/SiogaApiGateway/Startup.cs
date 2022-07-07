using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Refit;
using SiogaApiGateway.Handler;
using SiogaApiGateway.RestClients;
using SiogaApiGateway.Service.Contracts;
using SiogaApiGateway.Service.Implementation;

namespace SiogaApiGateway
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
            services.AddOcelot()
                    .AddPolly()
                    .AddDelegatingHandler<AuthHandler>()
                    .AddDelegatingHandler<SiogaHandler>()
                    .AddDelegatingHandler<ComunesHandler>()
                    .AddDelegatingHandler<RecaudacionHandler>()
                    .AddDelegatingHandler<SubvencionHandler>()
                    .AddDelegatingHandler<CopasejuHandler>()
                    .AddDelegatingHandler<SiogaPublicHandler>()
                    .AddDelegatingHandler<ReporteHandler>()
                    .AddDelegatingHandler<ReporteCopasejuHandler>()
                    .AddDelegatingHandler<FileServerHandler>()
                .AddAppConfiguration();

            services.AddSingleton<IAutorizationService, AutorizationService>();
            services.AddTransient<RefitHandler>();

            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());

            services.AddSwaggerForOcelot(Configuration);

            services.AddControllers();
            services.AddCors();

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:SecretKeyJWT").Value);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                               .AddJwtBearer("ApiSecurity", x =>
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


            services.AddRefitClient<IAuthorizationAPI>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Services:siogaAuthorization:DownstreamPath").Value))
                .AddHttpMessageHandler<RefitHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();
            app.UsePathBase("/gateway");
            app.UseSwagger();

            var appSettings = new AppSettings();
            app.ApplicationServices.GetService<IConfiguration>()
                .GetSection("AppSettings")
                .Bind(appSettings);

            app.UseCors(x => x
                        .WithOrigins(appSettings.AllowedSiogaOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();

            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.DownstreamSwaggerEndPointBasePath = "gateway/swagger/docs";

                opt.RoutePrefix = string.Empty;
            })
            .UseOcelot()
            .Wait();

            app.Run(context => context.Response.WriteAsync("ApiGateway"));

        }

    }
}
