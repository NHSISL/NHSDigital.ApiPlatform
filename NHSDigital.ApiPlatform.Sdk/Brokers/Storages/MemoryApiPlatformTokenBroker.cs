// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Storages
{
    internal sealed class MemoryApiPlatformTokenBroker : IApiPlatformTokenBroker
    {
        private readonly object locker = new();

        private string? accessToken;
        private DateTimeOffset? accessExpiresAtUtc;

        private string? refreshToken;
        private DateTimeOffset? refreshExpiresAtUtc;

        public ValueTask StoreAccessTokenAsync(
            string accessToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.accessToken = accessToken;
                this.accessExpiresAtUtc = expiresAtUtc;
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetAccessTokenAsync(
            CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                return ValueTask.FromResult((this.accessToken, this.accessExpiresAtUtc));
            }
        }

        public ValueTask ClearAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.accessToken = null;
                this.accessExpiresAtUtc = null;
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask StoreRefreshTokenAsync(
            string refreshToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.refreshToken = refreshToken;
                this.refreshExpiresAtUtc = expiresAtUtc;
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetRefreshTokenAsync(
            CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                return ValueTask.FromResult((this.refreshToken, this.refreshExpiresAtUtc));
            }
        }

        public ValueTask ClearRefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            lock (this.locker)
            {
                this.refreshToken = null;
                this.refreshExpiresAtUtc = null;
            }

            return ValueTask.CompletedTask;
        }
    }
}
