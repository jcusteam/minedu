using AutoMapper;
using RecaudacionApiRegistroLinea.Application.Query;
using RecaudacionApiRegistroLinea.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RecaudacionApiRegistroLinea.Application.Command;
using Refit;
using RecaudacionApiRegistroLinea.Clients;
using System;
using System.Net;
using RecaudacionApiReciboIngreso.Helpers;
using Microsoft.AspNetCore.Mvc;
using RecaudacionApiRegistroLinea.Helpers;
using FluentValidation.AspNetCore;
using RecaudacionApiReciboIngreso.Handler;

namespace RecaudacionApiRegistroLinea
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
            services.AddDbContext<RegistroLineaContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

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

            services.AddMediatR(typeof(FindByIdRegistroLineaHandler.Handler).Assembly);
            services.AddAutoMapper(typeof(AddRegistroLineaHandler.Handler));
            services.AddAutoMapper(typeof(FindByIdRegistroLineaHandler.Handler));
            services.AddScoped<IRegistroLineaRepository, RegistroLineaRepository>();
            services.AddScoped<IRegistroLineaDetalleRepository, RegistroLineaDetalleRepository>();
            services.AddTransient<RefitHandler>();

            services.AddRefitClient<IBancoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:BancoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IClasificadorIngresoAPI>()
                   .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ClasificadorIngresoApi:Url").Value))
                   .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IClienteAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ClienteApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ICuentaCorrienteAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:CuentaCorrienteApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IDepositoBancoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:DepositoBancoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IESindadExpedienteAPI>()
                  .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ESinadExpedienteApi:Url").Value))
                  .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IEstadoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:EstadoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IPideAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:PideApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IReciboIngresoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:ReciboIngresoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITipoDocumentoAPI>()
                  .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoDocumentoApi:Url").Value))
                  .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<ITipoReciboIngresoAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:TipoReciboIngresoApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            services.AddRefitClient<IUnidadEjecutoraAPI>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:UnidadEjecutoraApi:Url").Value))
                    .AddHttpMessageHandler<RefitHandler>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", new OpenApiInfo { Title = "Registro Línea API", Version = "v1" });
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
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Registro Línea API V1");
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
