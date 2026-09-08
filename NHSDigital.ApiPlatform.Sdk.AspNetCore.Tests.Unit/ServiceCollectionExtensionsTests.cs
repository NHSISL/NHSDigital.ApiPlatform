// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Unit
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ShouldRegisterHttpContextAccessorOnAddApiPlatformSdkAspNetCore()
        {
            // given
            IServiceCollection services = new ServiceCollection();

            // when
            services.AddApiPlatformSdkAspNetCore();

            // then
            services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IHttpContextAccessor));
        }

        [Fact]
        public void ShouldRegisterSessionStateBrokerOnAddApiPlatformSdkAspNetCore()
        {
            // given
            IServiceCollection services = new ServiceCollection();

            // when
            services.AddApiPlatformSdkAspNetCore();

            // then
            ServiceDescriptor actualDescriptor = services.Single(descriptor =>
                descriptor.ServiceType == typeof(IApiPlatformStateBroker));

            actualDescriptor.ImplementationType.Should().Be(typeof(SessionApiPlatformStateBroker));
            actualDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }

        [Fact]
        public void ShouldRegisterSessionTokenBrokerOnAddApiPlatformSdkAspNetCore()
        {
            // given
            IServiceCollection services = new ServiceCollection();

            // when
            services.AddApiPlatformSdkAspNetCore();

            // then
            ServiceDescriptor actualDescriptor = services.Single(descriptor =>
                descriptor.ServiceType == typeof(IApiPlatformTokenBroker));

            actualDescriptor.ImplementationType.Should().Be(typeof(SessionApiPlatformTokenBroker));
            actualDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }

        [Fact]
        public void ShouldReturnSameServiceCollectionOnAddApiPlatformSdkAspNetCore()
        {
            // given
            IServiceCollection services = new ServiceCollection();

            // when
            IServiceCollection actualServices = services.AddApiPlatformSdkAspNetCore();

            // then
            actualServices.Should().BeSameAs(services);
        }
    }
}
