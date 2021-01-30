﻿using AutoMapper;
using BeepBackend.Data;
using BeepBackend.Helpers;
using BeepBackend.Mailing;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Text;
using BeepBackend.Articles.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Utrix.WebLib;

namespace BeepBackend
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<BeepDbContext>();
            builder.AddDefaultTokenProviders();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddCors();
            services.AddMvc(ConfigureMvc)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1,0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ApiVersionReader = new HeaderApiVersionReader("X-ApiVersion");
            });

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.EventsType = typeof(BeepBearerEvents);
                });

            services.AddDbContext<BeepDbContext>(o =>
                o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddTransient<IAuthorizationHandler, HasChangePermissionRequirementHandler>();
            services.AddTransient<IAuthorizationHandler, HasEnvironmentPermissionRequirementHandler>();
            services.AddSingleton<IPermissionsCache, PermissionsCache>();
            services.AddTransient<BeepBearerEvents>();
            services.AddTransient<IBeepMailer, Mailer>();
            services.AddScoped<IShoppingListRepo, ShoppingListRepo>();
            services.AddHostedService<DemoUserCleanup>();
            services.AddHostedService<PermissionsCacheCleanup>();

            if (_environment.IsDevelopment())
            {
                services.AddTransient<IMailerClient, SmtpMailerClient>();
            }
            else
            {
                services.AddTransient<IMailerClient, SendGridMailerClient>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var handler = context.Features.Get<IExceptionHandlerFeature>();
                        if (handler != null)
                        {
                            context.Response.AddApplicationError(handler.Error.Message);
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(handler.Error, new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }));
                        }
                    });
                });
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var handler = context.Features.Get<IExceptionHandlerFeature>();
                        if (handler != null)
                        {
                            context.Response.AddApplicationError(handler.Error.Message);
                            await context.Response.WriteAsync(handler.Error.Message);
                        }
                    });
                });
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMiddleware<PermissionsChangedMiddleware>();

            if (_environment.IsProduction())
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Index", "Fallback");
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }

        }

        private static void ConfigureMvc(MvcOptions options)
        {
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        }
    }
}
