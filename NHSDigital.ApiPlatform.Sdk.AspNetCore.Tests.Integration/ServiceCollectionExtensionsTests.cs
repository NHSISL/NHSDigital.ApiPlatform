// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Integration
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ShouldResolveApiPlatformClientFromTheComposedAspNetCoreContainer()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            // when
            var actualClient = serviceScope.ServiceProvider.GetRequiredService<IApiPlatformClient>();

            // then
            actualClient.CareIdentityServiceClient.Should().NotBeNull();
            actualClient.PersonalDemographicsServiceClient.Should().NotBeNull();
        }

        [Fact]
        public void ShouldOverrideTheInMemoryStorageBrokersWithSessionBackedOnes()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            // when
            var actualStateBroker =
                serviceScope.ServiceProvider.GetRequiredService<IApiPlatformStateBroker>();

            var actualTokenBroker =
                serviceScope.ServiceProvider.GetRequiredService<IApiPlatformTokenBroker>();

            // then
            actualStateBroker.GetType().Name.Should().Be("SessionApiPlatformStateBroker");
            actualTokenBroker.GetType().Name.Should().Be("SessionApiPlatformTokenBroker");
        }

        [Fact]
        public async Task ShouldRoundTripTheCsrfStateThroughTheSessionAsync()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            var stateBroker =
                serviceScope.ServiceProvider.GetRequiredService<IApiPlatformStateBroker>();

            string randomState = Guid.NewGuid().ToString();

            // when
            await stateBroker.StoreCsrfStateAsync(randomState);

            // then
            string actualState = await stateBroker.GetCsrfStateAsync();
            actualState.Should().Be(randomState);
        }

        [Fact]
        public async Task ShouldRoundTripTheAccessTokenThroughTheSessionAsync()
        {
            // given
            ServiceProvider serviceProvider = BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            var tokenBroker =
                serviceScope.ServiceProvider.GetRequiredService<IApiPlatformTokenBroker>();

            string randomAccessToken = Guid.NewGuid().ToString();
            DateTimeOffset expiresAtUtc = DateTimeOffset.UtcNow.AddHours(1);

            // when
            await tokenBroker.StoreAccessTokenAsync(randomAccessToken, expiresAtUtc);

            // then
            var (actualToken, _) = await tokenBroker.GetAccessTokenAsync();
            actualToken.Should().Be(randomAccessToken);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            ApiPlatformConfigurations configurations =
                ConfigurationProvider.GetApiPlatformConfigurations();

            var httpContext = new DefaultHttpContext
            {
                Session = new IntegrationSession()
            };

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor { HttpContext = httpContext });

            services.AddApiPlatformSdkCore(configurations);
            services.AddApiPlatformSdkAspNetCore();

            return services.BuildServiceProvider();
        }

        private sealed class IntegrationSession : ISession
        {
            private readonly Dictionary<string, byte[]> store = new Dictionary<string, byte[]>();

            public bool IsAvailable => true;
            public string Id => "integration-session";
            public IEnumerable<string> Keys => this.store.Keys;

            public void Clear() => this.store.Clear();

            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => this.store.Remove(key);

            public void Set(string key, byte[] value) => this.store[key] = value;

            public bool TryGetValue(string key, out byte[] value) => this.store.TryGetValue(key, out value);
        }
    }
}
