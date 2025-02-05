﻿using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FwksLabs.Libs.Core.Extensions;
using FwksLabsCorsOptions = FwksLabs.Libs.Core.Security.Options.CorsOptions;

namespace FwksLabs.Libs.AspNetCore.Configuration.Options;

public sealed class CustomCorsOptions(
    FwksLabsCorsOptions corsOptions,
    ILogger<CustomCorsOptions> logger) : IConfigureOptions<CorsOptions>
{
    public void Configure(CorsOptions options)
    {
        logger.LogInformation("Configuring '{OptionsType}'", GetType().Name.SpaceTitleCase());

        foreach (var settings in corsOptions.Policies)
        {
            if (options.GetPolicy(settings.Name) != null)
            {
                logger.LogInformation($"Policy '{{PolicyName}}' already exists. Skipping", settings.Name);

                continue;
            }

            options.AddPolicy(settings.Name, policy =>
            {
                policy.WithOrigins(settings.Origins);
                policy.WithMethods(settings.Methods.Split(','));

                if (settings.ExposedHeaders.Length > 0)
                    policy.WithExposedHeaders(settings.ExposedHeaders);

                if (settings.Headers.Length > 0)
                    policy.WithHeaders(settings.Headers);
            });
        }
    }
}