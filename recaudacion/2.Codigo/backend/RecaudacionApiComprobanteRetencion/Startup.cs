using AutoMapper;
using RecaudacionApiComprobanteRetencion.Application.Query;
using RecaudacionApiComprobanteRetencion.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecaudacionApiComprobanteRetencion.Application.Command;
using Refit;
using RecaudacionApiComprobanteRetencion.Clients;
using System;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobanteRetencion.Helpers;
using FluentValidation.AspNetCore;
using System.Net;
using RecaudacionApiDepositoBanco.Handler;

namespace RecaudacionApiComprobanteRetencion
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
            services.AddDbContext<ComprobanteRetencionContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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

            services.AddMediatR(typeof(FindByIdComprobanteRetencionHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(AddComprobanteRetencionHandler.Handler));
            services.AddAutoMapper(typeof(FindByIdComprobanteRetencionHandler.Handler));
            services.AddScoped<IComprobanteRetencionRepository, ComprobanteRetencionRepository>();
            services.AddScoped<IComprobanteRetencionDetalleRepository, ComprobanteRetencionDetalleRepository>();
            services.AddTransient<RefitHandler>();

            services.AddRefitClient<IClienteAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ClienteApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IEstadoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:EstadoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITipoDocumentoAPI>()
                 .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoDocumentoApi:Url").Value))
                 .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IUnidadEjecutoraAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:UnidadEjecutoraApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IComprobanteEmisorAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ComprobanteEmisorApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITipoComprobantePagoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoComprobantePagoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IOseSunatAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:OseSunatApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = "Comprobante Retenci�n API", Version = "v1" });
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
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Comprobante Emisor API V1");
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
