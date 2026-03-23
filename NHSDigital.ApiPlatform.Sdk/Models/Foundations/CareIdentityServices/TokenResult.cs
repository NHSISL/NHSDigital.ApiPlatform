// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text.Json.Serialization;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices
{
    public sealed class TokenResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public string ExpiresIn { get; set; } = "0";

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token_expires_in")]
        public string RefreshTokenExpiresIn { get; set; } = "0";

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
    }
}
