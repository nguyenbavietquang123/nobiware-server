/*
 * Copyright (c) 2019-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Spark.Engine;
using Spark.Engine.Extensions;
using Spark.Mongo.Extensions;
using Spark.Web.Data;
using Spark.Web.Models.Config;
using Spark.Web.Services;
using Spark.Web.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Spark.Web.Models;
using Spark.Web.Utilities;

namespace Spark.Web;

public class Startup
{
    private readonly ILogger<Startup> _logger;

    public Startup(IConfiguration configuration, ILogger<Startup> logger)
    {
        Configuration = configuration;
        _logger = logger;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Retain previous behavior of DateTime values defaulting to local time.
        // FIXME: Remove this in the future.
        services.AddControllersWithViews(options =>
            options.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>());

        // Bind to Spark and store settings from appSettings.json
        SparkSettings sparkSettings = new SparkSettings();
        Configuration.Bind("SparkSettings", sparkSettings);
        services.AddSingleton<SparkSettings>(sparkSettings);

        // IntrospectSettings introspectSettings = new IntrospectSettings();
        // Configuration.GetSection("Introspection", introspectSettings);
        // services.AddSingleton<IntrospectSettings>(introspectSettings);
        services.Configure<IntrospectSettings>(Configuration.GetSection("Introspection"));

        StoreSettings storeSettings = new StoreSettings();
        Configuration.Bind("StoreSettings", storeSettings);
        //Console.WriteLine("⚙️ MongoDB Connection String: " + storeSettings.ConnectionString);
        // Read examples settings from config
        ExamplesSettings examplesSettings = new ExamplesSettings();
        Configuration.Bind("ExamplesSettings", examplesSettings);
        services.Configure<ExamplesSettings>(options => Configuration.GetSection("ExamplesSettings").Bind(options));
        services.AddSingleton<ExamplesSettings>(examplesSettings);

        // Configure cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/fhir+json", "application/fhir+xml" });
        });

        // Add database context for user administration
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
        // );
        
        // Add Identity management
        // services.AddIdentity<IdentityUser, IdentityRole>()
        //     .AddRoles<IdentityRole>()
        //     .AddDefaultUI()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //         .AddJwtBearer(options =>
        //         {
        //             options.Authority = "http://localhost:8080/realms/quang-fhir-server";
        //             options.Audience = "fhir-client";
        //             options.RequireHttpsMetadata = false;
        //             options.TokenValidationParameters = new TokenValidationParameters
        //             {
        //                 ValidateIssuer = true,
        //                 ValidateActor = true,
        //                 ValidateLifetime = true,
        //                 ValidateIssuerSigningKey = true
        //             };
        //         });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            // Skipping cert checking, remove later when keycloak using valid cert (dev only)
            // options.BackchannelHttpHandler = new HttpClientHandler
            // {
            //     ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            // };
            options.Audience = Configuration["Authentication:Audience"];
            options.MetadataAddress = Configuration["Authentication:MetadataAddress"]!;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["Authentication:ValidIssuer"]
            };
        });

        services.AddAuthorization();

        // Set up a default policy for CORS that accepts any origin, method and header.
        // only for test purposes.
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            }));

        // Sets up the MongoDB store
        services.AddMongoFhirStore(storeSettings);

        // AddFhir also calls AddMvcCore
        services.AddFhir(sparkSettings);

        services.AddTransient<ServerMetadata>();

        // AddMvc needs to be called since we are using a Home page that is reliant on the full MVC framework
        services.AddMvc(options =>
        {
            options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();
            options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
            // We remove StringOutputFormatter to make Swagger happy by not 
            // showing text/plain in the list of available media types.
            options.OutputFormatters.RemoveType<StringOutputFormatter>();
            options.EnableEndpointRouting = false;
        });

        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            c.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(Configuration["KeyCloak:AuthorizationUrl"]),
                        Scopes = new Dictionary<string, string>
                        {
                            {"openid","openid"},
                            {"profile","profile"}

                        }
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer"
                    },
                    []
                }
            };

            c.AddSecurityRequirement(securityRequirement);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Spark API", Version = "v1" });
        });
        services.AddSignalR();
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
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseSwagger(

        );
        app.UseSwaggerUI(c =>
        {

            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spark API");
        });



        app.UseCors();
        //     app.Use(async (context, next) =>
        // {
        //     var sw = System.Diagnostics.Stopwatch.StartNew();

        //     // Try to get real client IP (X-Forwarded-For from reverse proxy, else fallback)
        //     var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
        //              ?? context.Connection.RemoteIpAddress?.ToString();

        //     var method = context.Request.Method;
        //     var path = context.Request.Path;
        //      _logger.LogInformation("Started request from {IP} - {Method} {Path}", ip, method, path);
        //     await next(); // call next middleware

        //     sw.Stop();
        //     var statusCode = context.Response.StatusCode;

        //     _logger.LogInformation("Request from {IP} - {Method} {Path} => {StatusCode} in {Elapsed}ms",
        //         ip, method, path, statusCode, sw.ElapsedMilliseconds);
        // });
        app.UseMiddleware<SessionTrackingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<MaintenanceHub>("/maintenanceHub");
        });

        // UseFhir also calls UseMvc
        app.UseFhir(r => r.MapRoute(name: "default", template: "{controller}/{action}/{id?}", defaults: new { controller = "Home", action = "Index" }));
    }
}