using RecaudacionApiOseSunat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecaudacionApiOseSunat.Services.Contracts;
using RecaudacionApiOseSunat.Services.Implementation;
using MediatR;
using RecaudacionApiOseSunat.Applicacion.Command;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiOseSunat.Helpers;
using FluentValidation.AspNetCore;
using System.Net;
using RecaudacionApiOseSunat.Handler;

namespace RecaudacionApiOseSunat
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

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressConsumesConstraintForFormFileParameters = true;
            });

            services.AddMvc(
                options =>
                {
                    options.Filters.Add(new ValidationActionFilter());
                }
            ).AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddTransient<IOseSunatService, OseSunatService>();
            services.AddSingleton(Configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddTransient<RefitHandler>();

            services.AddMediatR(typeof(SendComprobanteHandler.Handler).Assembly);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OSE SUNAT API", Version = "v1" });
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
            app.UseStatusCodePages(async context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await CustomResponse.NotFound(response);
                }
                else
                {
                    response.Redirect("/");
                }
            });

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "OSE SUNAT API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("MineduPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
