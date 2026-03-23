// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Storages
{
    public interface IApiPlatformTokenBroker
    {
        ValueTask StoreAccessTokenAsync(
            string accessToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default);

        ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetAccessTokenAsync(
            CancellationToken cancellationToken = default);

        ValueTask ClearAccessTokenAsync(CancellationToken cancellationToken = default);

        ValueTask StoreRefreshTokenAsync(
            string refreshToken,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default);

        ValueTask<(string? Token, DateTimeOffset? ExpiresAtUtc)> GetRefreshTokenAsync(
            CancellationToken cancellationToken = default);

        ValueTask ClearRefreshTokenAsync(CancellationToken cancellationToken = default);
    }
}
