using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiJWTManual.Services;
using Aplicacion.UnitOfWork;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ApiJWTManual.Extensions;
public static class ApplicationServiceExtension
{
    public static void ConfigureCors(this IServiceCollection services) =>
   services.AddCors(Options =>
   {
       Options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
   });
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
