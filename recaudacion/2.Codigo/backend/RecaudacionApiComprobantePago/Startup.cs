using AutoMapper;
using RecaudacionApiComprobantePago.Application.Query;
using RecaudacionApiComprobantePago.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecaudacionApiComprobantePago.Application.Command;
using Refit;
using RecaudacionApiComprobantePago.Clients;
using System;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiComprobantePago.Helpers;
using FluentValidation.AspNetCore;
using System.Net;
using RecaudacionApiComprobantePago.Handler;

namespace RecaudacionApiComprobantePago
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
            services.AddDbContext<ComprobantePagoContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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

            services.AddMediatR(typeof(FindByIdComprobantePagoHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(AddComprobantePagoHandler.Handler));
            services.AddAutoMapper(typeof(FindByIdComprobantePagoHandler.Handler));
            services.AddScoped<IComprobantePagoRepository, ComprobantePagoRepository>();
            services.AddScoped<IComprobantePagoDetalleRepository, ComprobantePagoDetalleRepository>();
            services.AddTransient<RefitHandler>();

            services.AddRefitClient<ICatalogoBienAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:CatalogoBienApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IClienteAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ClienteApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITarifarioAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TarifarioApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IComprobanteEmisorAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ComprobanteEmisorApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IOseSunatAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:OseSunatApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITipoComprobantePagoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoComprobantePagoApi:Url").Value))
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

            services.AddRefitClient<ICuentaCorrienteAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:CuentaCorrienteApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IDepositoBancoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:DepositoBancoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IPideAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:PideApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = "Comprobante Pago API", Version = "v1" });
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
