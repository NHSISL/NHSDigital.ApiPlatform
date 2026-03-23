// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Brokers.Storages
{
    internal static class SessionApiPlatformStorageKeys
    {
        internal const string CsrfState = "Nhs.ApiPlatform.CsrfState";
        internal const string AccessToken = "Nhs.ApiPlatform.AccessToken";
        internal const string AccessTokenExpiresAtUtc = "Nhs.ApiPlatform.AccessToken.ExpiresAtUtc";
        internal const string RefreshToken = "Nhs.ApiPlatform.RefreshToken";
        internal const string RefreshTokenExpiresAtUtc = "Nhs.ApiPlatform.RefreshToken.ExpiresAtUtc";
    }
}
