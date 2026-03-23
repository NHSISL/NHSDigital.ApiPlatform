// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Brokers.Cryptographies;
using NHSDigital.ApiPlatform.Sdk.Brokers.DateTimes;
using NHSDigital.ApiPlatform.Sdk.Brokers.Https;
using NHSDigital.ApiPlatform.Sdk.Brokers.Serializations;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices
{
    internal sealed partial class CareIdentityService : ICareIdentityService
    {
        private readonly ApiPlatformConfigurations configurations;
        private readonly IHttpBroker httpBroker;
        private readonly IJsonBroker jsonBroker;
        private readonly ICryptoBroker cryptoBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IApiPlatformStateBroker stateBroker;
        private readonly IApiPlatformTokenBroker tokenBroker;

        public CareIdentityService(
            ApiPlatformConfigurations configurations,
            IHttpBroker httpBroker,
            IJsonBroker jsonBroker,
            ICryptoBroker cryptoBroker,
            IDateTimeBroker dateTimeBroker,
            IApiPlatformStateBroker stateBroker,
            IApiPlatformTokenBroker tokenBroker)
        {
            this.configurations = configurations;
            this.httpBroker = httpBroker;
            this.jsonBroker = jsonBroker;
            this.cryptoBroker = cryptoBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.stateBroker = stateBroker;
            this.tokenBroker = tokenBroker;
        }
        public ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            string csrfState = this.cryptoBroker.CreateUrlSafeState();
            await this.stateBroker.StoreCsrfStateAsync(csrfState, cancellationToken);
            CareIdentityConfigurations careIdentityConfigurations = this.configurations.CareIdentity;

            // CIS2 only supports these parameters (no PKCE)
            string url =
                $"{careIdentityConfigurations.AuthEndpoint}" +
                $"?client_id={careIdentityConfigurations.ClientId}" +
                $"&redirect_uri={Uri.EscapeDataString(careIdentityConfigurations.RedirectUri)}" +
                $"&response_type=code" +
                $"&state={csrfState}";

            if (string.IsNullOrWhiteSpace(careIdentityConfigurations.AcrValues) is false)
            {
                url += $"&acr_values={careIdentityConfigurations.AcrValues}";
            }

            return url;
        });

        public ValueTask LogoutAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.stateBroker.ClearCsrfStateAsync(cancellationToken);
            await this.tokenBroker.ClearAccessTokenAsync(cancellationToken);
            await this.tokenBroker.ClearRefreshTokenAsync(cancellationToken);
        });

        public ValueTask CallbackAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnCallback(code, state);

            string? expectedState = await this.stateBroker.GetCsrfStateAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(expectedState) ||
                string.Equals(state, expectedState, StringComparison.Ordinal) is false)
            {
                throw new InvalidOperationException("Invalid state parameter.");
            }

            await this.stateBroker.ClearCsrfStateAsync(cancellationToken);

            TokenResult token = await ExchangeCodeForTokenAsync(code, cancellationToken);
            _ = await GetUserInfoAsync(token.AccessToken, cancellationToken);
            _ = int.TryParse(token.ExpiresIn, out int accessExpiresInSeconds);
            _ = int.TryParse(token.RefreshTokenExpiresIn, out int refreshExpiresInSeconds);

            DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();
            DateTimeOffset accessExpiresAtUtc = now.AddSeconds(Math.Max(accessExpiresInSeconds, 0));
            DateTimeOffset refreshExpiresAtUtc = now.AddSeconds(Math.Max(refreshExpiresInSeconds, 0));

            await this.tokenBroker.StoreAccessTokenAsync(token.AccessToken, accessExpiresAtUtc, cancellationToken);

            if (string.IsNullOrWhiteSpace(token.RefreshToken) is false)
            {
                await this.tokenBroker.StoreRefreshTokenAsync(
                    token.RefreshToken,
                    refreshExpiresAtUtc,
                    cancellationToken);
            }
        });

        public ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            var (accessToken, accessExpiresAtUtc) =
                await this.tokenBroker.GetAccessTokenAsync(cancellationToken);

            DateTimeOffset now = this.dateTimeBroker.GetCurrentDateTimeOffset();

            if (string.IsNullOrWhiteSpace(accessToken) is false &&
                accessExpiresAtUtc is not null &&
                accessExpiresAtUtc.Value > now.AddSeconds(60))
            {
                return accessToken!;
            }

            var (refreshToken, refreshExpiresAtUtc) =
                await this.tokenBroker.GetRefreshTokenAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshToken) ||
                refreshExpiresAtUtc is null ||
                refreshExpiresAtUtc.Value <= now)
            {
                return string.Empty;
            }

            TokenResult refreshed =
                await ExchangeRefreshTokenForTokenAsync(refreshToken!, cancellationToken);

            _ = int.TryParse(refreshed.ExpiresIn, out int newAccessExpiresInSeconds);
            _ = int.TryParse(refreshed.RefreshTokenExpiresIn, out int newRefreshExpiresInSeconds);

            DateTimeOffset newAccessExpiresAtUtc = now.AddSeconds(Math.Max(newAccessExpiresInSeconds, 0));
            DateTimeOffset newRefreshExpiresAtUtc = now.AddSeconds(Math.Max(newRefreshExpiresInSeconds, 0));

            await this.tokenBroker.StoreAccessTokenAsync(
                refreshed.AccessToken,
                newAccessExpiresAtUtc,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshed.RefreshToken) is false)
            {
                await this.tokenBroker.StoreRefreshTokenAsync(
                    refreshed.RefreshToken,
                    newRefreshExpiresAtUtc,
                    cancellationToken);
            }

            ValidateAccessToken(refreshed.AccessToken);

            return refreshed.AccessToken;
        });

        public ValueTask<NhsUserInfo> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken) =>
        TryCatch(async () =>
        {
            ValidateOnGetUserInfo(accessToken);
            CareIdentityConfigurations careIdentityConfigurations = this.configurations.CareIdentity;

            var response = await this.httpBroker.GetAsync(
                careIdentityConfigurations.UserInfoEndpoint,
                request => request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken),
                cancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            NhsUserInfo? userInfo = this.jsonBroker.Deserialize<NhsUserInfo>(json);

            return userInfo ?? throw new InvalidOperationException("UserInfo endpoint returned an invalid payload.");
        });

        private ValueTask<TokenResult> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken) =>
        TryCatch(async () =>
        {
            ValidateOnExchangeCodeForToken(code);
            CareIdentityConfigurations careIdentityConfigurations = this.configurations.CareIdentity;

            var formValues = new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", careIdentityConfigurations.RedirectUri),
                new KeyValuePair<string, string>("client_id", careIdentityConfigurations.ClientId),
                new KeyValuePair<string, string>("client_secret", careIdentityConfigurations.ClientSecret)
            };

            var response = await this.httpBroker
                .PostFormAsync(careIdentityConfigurations.TokenEndpoint, formValues, cancellationToken);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            TokenResult? token = this.jsonBroker.Deserialize<TokenResult>(json);

            return token ?? throw new InvalidOperationException("Token endpoint returned an invalid payload.");
        });

        private ValueTask<TokenResult> ExchangeRefreshTokenForTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken) =>
         TryCatch(async () =>
         {
             ValidateOnExchangeRefreshTokenForToken(refreshToken);

             CareIdentityConfigurations careIdentityConfigurations = this.configurations.CareIdentity;

             var formValues = new[]
             {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("client_id", careIdentityConfigurations.ClientId),
                    new KeyValuePair<string, string>("client_secret", careIdentityConfigurations.ClientSecret)
                };

             var response = await this.httpBroker
                 .PostFormAsync(careIdentityConfigurations.TokenEndpoint, formValues, cancellationToken);

             response.EnsureSuccessStatusCode();

             string json = await response.Content.ReadAsStringAsync(cancellationToken);
             TokenResult? token = this.jsonBroker.Deserialize<TokenResult>(json);

             return token ?? throw new InvalidOperationException("Token endpoint returned an invalid payload.");
         });
    }
}
