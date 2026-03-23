// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSDigital.ApiPlatform.Sdk.Models.Configurations
{
    public class CareIdentityConfigurations
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;

        public string AuthEndpoint { get; set; } = string.Empty;
        public string TokenEndpoint { get; set; } = string.Empty;
        public string UserInfoEndpoint { get; set; } = string.Empty;

        // Optional - e.g. "aal3"
        public string? AcrValues { get; set; }
    }
}
