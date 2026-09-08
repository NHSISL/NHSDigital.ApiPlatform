// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ShouldResolveApiPlatformClientFromTheComposedContainer()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();

            // when
            var actualClient = serviceProvider.GetRequiredService<IApiPlatformClient>();

            // then
            actualClient.CareIdentityServiceClient.Should().NotBeNull();
            actualClient.PersonalDemographicsServiceClient.Should().NotBeNull();
        }

        [Fact]
        public void ShouldResolveCareIdentityServiceClientFromTheComposedContainer()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();

            // when
            var actualClient = serviceProvider.GetRequiredService<ICareIdentityServiceClient>();

            // then
            actualClient.Should().NotBeNull();
        }

        [Fact]
        public void ShouldResolvePersonalDemographicsServiceClientFromTheComposedContainer()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();

            // when
            var actualClient = serviceProvider.GetRequiredService<IPersonalDemographicsServiceClient>();

            // then
            actualClient.Should().NotBeNull();
        }

        [Fact]
        public void ShouldFallBackToInMemoryStorageBrokersWhenNoneAreSupplied()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();

            // when
            var actualStateBroker = serviceProvider.GetRequiredService<IApiPlatformStateBroker>();
            var actualTokenBroker = serviceProvider.GetRequiredService<IApiPlatformTokenBroker>();

            // then
            actualStateBroker.GetType().Name.Should().Be("MemoryApiPlatformStateBroker");
            actualTokenBroker.GetType().Name.Should().Be("MemoryApiPlatformTokenBroker");
        }

        [Fact]
        public void ShouldShareStorageBrokersAcrossResolutions()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();

            // when
            var firstTokenBroker = serviceProvider.GetRequiredService<IApiPlatformTokenBroker>();
            var secondTokenBroker = serviceProvider.GetRequiredService<IApiPlatformTokenBroker>();

            // then
            firstTokenBroker.Should().BeSameAs(secondTokenBroker);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            ApiPlatformConfigurations configurations =
                ConfigurationProvider.GetApiPlatformConfigurations();

            IServiceCollection services = new ServiceCollection();
            services.AddApiPlatformSdkCore(configurations);
            services.AddApiPlatformSdkInMemoryStorage();

            return services.BuildServiceProvider();
        }
    }
}
