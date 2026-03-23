// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NHSDigital.ApiPlatform.Sdk.Brokers.Cryptographies;
using NHSDigital.ApiPlatform.Sdk.Brokers.DateTimes;
using NHSDigital.ApiPlatform.Sdk.Brokers.Https;
using NHSDigital.ApiPlatform.Sdk.Brokers.Identifiers;
using NHSDigital.ApiPlatform.Sdk.Brokers.Serializations;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds;
using NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiPlatformSdkCore(
            this IServiceCollection services,
            ApiPlatformConfigurations apiPlatformConfigurations)
        {
			services.AddSingleton(apiPlatformConfigurations);
			services.AddSingleton<ICryptoBroker, CryptoBroker>();
			services.AddSingleton<IDateTimeBroker, DateTimeBroker>();
			services.AddSingleton<IIdentifierBroker, IdentifierBroker>();
			services.AddSingleton<IJsonBroker, JsonBroker>();
			services.AddHttpClient("NhsApiPlatform");
			services.AddTransient<IHttpBroker, HttpBroker>();
			services.AddScoped<ICareIdentityService, CareIdentityService>();
			services.AddScoped<ICareIdentityServiceProcessingService, CareIdentityServiceProcessingService>();
			services.AddTransient<ICareIdentityServiceClient, CareIdentityServiceClient>();
			services.AddScoped<IPdsService, PdsService>();
			services.AddScoped<IPdsOrchestrationService, PdsOrchestrationService>();
			services.AddTransient<IPersonalDemographicsServiceClient, PersonalDemographicsServiceClient>();
			services.TryAddTransient<IApiPlatformClient>(serviceProvider =>
				new ApiPlatformClient(
					serviceProvider.GetRequiredService<ICareIdentityServiceClient>(),
					serviceProvider.GetRequiredService<IPersonalDemographicsServiceClient>()));

			return services;
        }

        public static IServiceCollection AddApiPlatformSdkInMemoryStorage(this IServiceCollection services)
        {
            // Defaults for standalone. ASP.NET Core package will NOT call this.
            services.TryAddSingleton<IApiPlatformStateBroker, MemoryApiPlatformStateBroker>();
            services.TryAddSingleton<IApiPlatformTokenBroker, MemoryApiPlatformTokenBroker>();

            return services;
        }
    }
}