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
            // AddApiPlatformSdkInMemoryStorage uses TryAdd, so the session brokers only win if
            // AddApiPlatformSdkAspNetCore has already registered them. Registering the in-memory
            // ones here is what makes this assertion capable of failing.
            ServiceProvider serviceProvider = BuildServiceProvider(withInMemoryStorage: true);
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
        public void ShouldStillUseTheSessionBrokersWhenInMemoryStorageIsRegisteredFirst()
        {
            // given
            // Registration order does NOT matter here, contrary to what one might expect from
            // TryAdd: AddApiPlatformSdkAspNetCore uses AddScoped, which appends rather than
            // no-ops, and the last registration for a service is the one that resolves. So the
            // session brokers win either way, and a web host cannot accidentally end up on the
            // process-wide singletons by ordering these two calls the "wrong" way round.
            ApiPlatformConfigurations configurations =
                ConfigurationProvider.GetApiPlatformConfigurations();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor { HttpContext = CreateHttpContext() });

            services.AddApiPlatformSdkCore(configurations);
            services.AddApiPlatformSdkInMemoryStorage();
            services.AddApiPlatformSdkAspNetCore();

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            // when
            var actualStateBroker =
                serviceScope.ServiceProvider.GetRequiredService<IApiPlatformStateBroker>();

            // then
            actualStateBroker.GetType().Name.Should().Be("SessionApiPlatformStateBroker");
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

        private static DefaultHttpContext CreateHttpContext() =>
            new DefaultHttpContext
            {
                Session = new IntegrationSession()
            };

        private static ServiceProvider BuildServiceProvider(bool withInMemoryStorage = false)
        {
            ApiPlatformConfigurations configurations =
                ConfigurationProvider.GetApiPlatformConfigurations();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor { HttpContext = CreateHttpContext() });

            services.AddApiPlatformSdkCore(configurations);
            services.AddApiPlatformSdkAspNetCore();

            if (withInMemoryStorage)
            {
                services.AddApiPlatformSdkInMemoryStorage();
            }

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
