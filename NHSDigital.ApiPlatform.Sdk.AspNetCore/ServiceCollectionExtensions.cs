// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers ASP.NET Core specific storage brokers for the NHS Digital API Platform SDK.
        /// 
        /// This wiring enables per-user storage via <see cref="ISession"/>.
        /// The host application must also configure session middleware:
        /// - services.AddDistributedMemoryCache() (or your distributed cache)
        /// - services.AddSession(...)
        /// - app.UseSession()
        /// </summary>
        public static IServiceCollection AddApiPlatformSdkAspNetCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Override the SDK's in-memory defaults with session-backed implementations.
            services.AddScoped<IApiPlatformStateBroker, SessionApiPlatformStateBroker>();
            services.AddScoped<IApiPlatformTokenBroker, SessionApiPlatformTokenBroker>();

            return services;
        }
    }
}
