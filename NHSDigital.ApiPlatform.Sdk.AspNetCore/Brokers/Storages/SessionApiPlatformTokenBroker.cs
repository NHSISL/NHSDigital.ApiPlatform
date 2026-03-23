// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages
{
    internal sealed class SessionApiPlatformTokenBroker : IApiPlatformTokenBroker
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SessionApiPlatformTokenBroker(IHttpContextAccessor httpContextAccessor) =>
            this.httpContextAccessor = httpContextAccessor;

        public ValueTask StoreAccessTokenAsync(
            string accessToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            session.SetString(SessionApiPlatformStorageKeys.AccessToken, accessToken);
            session.SetString(
                SessionApiPlatformStorageKeys.AccessTokenExpiresAtUtc,
                expiresAtUtc.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

            return ValueTask.CompletedTask;
        }

        public ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetAccessTokenAsync(
            CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            string? token = session.GetString(SessionApiPlatformStorageKeys.AccessToken);
            DateTimeOffset? expiresAtUtc = ReadExpiresAtUtc(session, SessionApiPlatformStorageKeys.AccessTokenExpiresAtUtc);

            return ValueTask.FromResult((token, expiresAtUtc));
        }

        public ValueTask ClearAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            session.Remove(SessionApiPlatformStorageKeys.AccessToken);
            session.Remove(SessionApiPlatformStorageKeys.AccessTokenExpiresAtUtc);

            return ValueTask.CompletedTask;
        }

        public ValueTask StoreRefreshTokenAsync(
            string refreshToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            session.SetString(SessionApiPlatformStorageKeys.RefreshToken, refreshToken);
            session.SetString(
                SessionApiPlatformStorageKeys.RefreshTokenExpiresAtUtc,
                expiresAtUtc.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

            return ValueTask.CompletedTask;
        }

        public ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetRefreshTokenAsync(
            CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            string? token = session.GetString(SessionApiPlatformStorageKeys.RefreshToken);
            DateTimeOffset? expiresAtUtc = ReadExpiresAtUtc(session, SessionApiPlatformStorageKeys.RefreshTokenExpiresAtUtc);

            return ValueTask.FromResult((token, expiresAtUtc));
        }

        public ValueTask ClearRefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            ISession session = GetSessionOrThrow();

            session.Remove(SessionApiPlatformStorageKeys.RefreshToken);
            session.Remove(SessionApiPlatformStorageKeys.RefreshTokenExpiresAtUtc);

            return ValueTask.CompletedTask;
        }

        private static DateTimeOffset? ReadExpiresAtUtc(ISession session, string expiresKey)
        {
            string? expiresAt = session.GetString(expiresKey);

            if (string.IsNullOrWhiteSpace(expiresAt))
            {
                return null;
            }

            bool parsed = long.TryParse(expiresAt, NumberStyles.Integer, CultureInfo.InvariantCulture, out long seconds);
            return parsed ? DateTimeOffset.FromUnixTimeSeconds(seconds) : null;
        }

        private ISession GetSessionOrThrow()
        {
            HttpContext? httpContext = this.httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                throw new InvalidOperationException(
                    "No active HttpContext. Ensure this code runs within an ASP.NET Core request pipeline.");
            }

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
