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
using SiogaApiPide.Application.Query;
using SiogaApiPide.Clients;
using SiogaApiPide.Handler;
using SiogaApiPide.Service.Contracts;
using SiogaApiPide.Service.Implementation;

namespace SiogaApiPide
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
            services.AddControllers();
            services.AddMediatR(typeof(FindByDniReniecHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(FindByDniReniecHandler.Handler));
            services.AddAutoMapper(typeof(FindByNroRucSunatHandler.Handler));
            services.AddTransient<IMigracionService, MigracionService>();
            services.AddTransient<IReniecService, ReniecService>();
            services.AddTransient<ISunatService, SunatService>();
            services.AddTransient<IReniecWcfService, ReniecWcfService>();
            services.AddTransient<RefitReniecHandler>();
            services.AddTransient<RefitMigracionHandler>();
            services.AddTransient<RefitSunatHandler>();

            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());

            services.AddRefitClient<IReniecAPI>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ReniecApi:Url").Value))
                .AddHttpMessageHandler<RefitReniecHandler>();

            services.AddRefitClient<ISunatAPI>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:SunatApi:Url").Value))
                .AddHttpMessageHandler<RefitMigracionHandler>();

            services.AddRefitClient<IMigracionAPI>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:MigracionesApi:Url").Value))
                .AddHttpMessageHandler<RefitSunatHandler>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minedu PIDE API", Version = "v1" });
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
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Minedu PIDE API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthorization();

            app.UseCors("MineduPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
