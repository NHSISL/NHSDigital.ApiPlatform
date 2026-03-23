// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages
{
    internal sealed class SessionApiPlatformStateBroker : IApiPlatformStateBroker
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SessionApiPlatformStateBroker(IHttpContextAccessor httpContextAccessor) =>
            this.httpContextAccessor = httpContextAccessor;

        public ValueTask StoreCsrfStateAsync(string state, CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();
            session.SetString(SessionApiPlatformStorageKeys.CsrfState, state);

            return ValueTask.CompletedTask;
        }

        public ValueTask<string?> GetCsrfStateAsync(CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();
            string? state = session.GetString(SessionApiPlatformStorageKeys.CsrfState);

            return ValueTask.FromResult(state);
        }

        public ValueTask ClearCsrfStateAsync(CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();
            session.Remove(SessionApiPlatformStorageKeys.CsrfState);

            return ValueTask.CompletedTask;
        }

        private ISession GetSessionOrThrow()
        {
            HttpContext? httpContext = this.httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                throw new InvalidOperationException(
                    "No active HttpContext. Ensure this code runs within an ASP.NET Core request pipeline.");
            }

            // Accessing Session will throw if session middleware is not configured.
            try
            {
                return httpContext.Session;
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException(
                    "Session is not available. Ensure you have configured session services (services.AddSession) and middleware (app.UseSession).",
                    exception);
            }
        }
    }
}
