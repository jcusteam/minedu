using AutoMapper;
using RecaudacionApiParametro.Application.Query;
using RecaudacionApiParametro.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecaudacionApiParametro.Application.Command;
using Refit;
using RecaudacionApiParametro.Clients;
using System;
using System.Net;
using RecaudacionApiParametro.Helpers;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using RecaudacionApiLiquidacion.Handler;

namespace RecaudacionApiParametro
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
            services.AddDbContext<ParametroContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(opt =>
            {
                opt.AddPolicy("MineduPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                });
            });

            services.AddControllers().AddNewtonsoftJson();

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

            services.AddMediatR(typeof(FindByIdParametroHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(AddParametroHandler.Handler));
            services.AddAutoMapper(typeof(FindByIdParametroHandler.Handler));
            services.AddScoped<IParametroRepository, ParametroRepository>();
            services.AddTransient<RefitHandler>();

            services.AddRefitClient<ITipoDocumentoAPI>()
                              .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoDocumentoApi:Url").Value))
                              .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IUnidadEjecutoraAPI>()
                              .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:UnidadEjecutoraApi:Url").Value))
                              .AddHttpMessageHandler<RefitHandler>();


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = "Par??metros API", Version = "v1" });
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
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Par??metros API V1");
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
