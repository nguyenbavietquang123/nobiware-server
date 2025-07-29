﻿/*
 * Copyright (c) 2019-2025, Incendi <info@incendi.no>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spark.Web.Data;
using Microsoft.AspNetCore.Authentication;

namespace Spark.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateWebHostBuilder(args).Build();
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .AddUserSecrets<Startup>().Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // var context = services.GetRequiredService<ApplicationDbContext>();
                // var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                //ApplicationDbInitializer.SeedAdmin(context, userManager, config);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, 6000, listenOptions =>
    {
    });
})
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
            });
}