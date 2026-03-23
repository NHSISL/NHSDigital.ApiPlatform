// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;

namespace NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms
{
    public sealed class ApiPlatformClient : IApiPlatformClient
    {
        // Standalone/quick-start constructor (no DI knowledge required)
        // Uses in-memory storage defaults.
        public ApiPlatformClient(ApiPlatformConfigurations apiPlatformConfigurations)
        {
            IServiceProvider serviceProvider =
                BuildStandaloneServiceProvider(
                    apiPlatformConfigurations,
                    apiPlatformStateBroker: null,
                    apiPlatformTokenBroker: null);

            InitializeClients(serviceProvider);
        }

        // Standalone factory for non-DI hosts that want custom storage brokers.
        // Falls back to in-memory brokers if none are supplied.
        public static IApiPlatformClient Create(
            ApiPlatformConfigurations apiPlatformConfigurations,
            IApiPlatformStateBroker apiPlatformStateBroker = null,
            IApiPlatformTokenBroker apiPlatformTokenBroker = null)
        {
            IServiceProvider serviceProvider =
                BuildStandaloneServiceProvider(
                    apiPlatformConfigurations,
                    apiPlatformStateBroker,
                    apiPlatformTokenBroker);

            return serviceProvider.GetRequiredService<IApiPlatformClient>();
        }

        // DI constructor (ASP.NET Core will use this)
        public ApiPlatformClient(
            ICareIdentityServiceClient careIdentityServiceClient,
            IPersonalDemographicsServiceClient personalDemographicsServiceClient)
        {
            CareIdentityServiceClient = careIdentityServiceClient;
            PersonalDemographicsServiceClient = personalDemographicsServiceClient;
        }

        public ICareIdentityServiceClient CareIdentityServiceClient { get; private set; }
        public IPersonalDemographicsServiceClient PersonalDemographicsServiceClient { get; private set; }

        private void InitializeClients(IServiceProvider serviceProvider)
        {
            CareIdentityServiceClient =
                serviceProvider.GetRequiredService<ICareIdentityServiceClient>();

            PersonalDemographicsServiceClient =
                serviceProvider.GetRequiredService<IPersonalDemographicsServiceClient>();
        }

        private static IServiceProvider BuildStandaloneServiceProvider(
            ApiPlatformConfigurations apiPlatformConfigurations,
            IApiPlatformStateBroker apiPlatformStateBroker,
            IApiPlatformTokenBroker apiPlatformTokenBroker)
        {
            IServiceCollection services = new ServiceCollection();

            // Shared core registrations:
            services.AddApiPlatformSdkCore(apiPlatformConfigurations);

            // Optional custom brokers (non-DI hosts):
            if (apiPlatformStateBroker is not null)
            {
                services.AddSingleton<IApiPlatformStateBroker>(_ => apiPlatformStateBroker);
            }

            if (apiPlatformTokenBroker is not null)
            {
                services.AddSingleton<IApiPlatformTokenBroker>(_ => apiPlatformTokenBroker);
            }

            // Standalone defaults only (applies if custom brokers were not provided):
            services.AddApiPlatformSdkInMemoryStorage();

            return services.BuildServiceProvider();
        }
    }
}