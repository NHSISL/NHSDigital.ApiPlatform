// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices
{
    public sealed class NhsUserInfo
    {
        [JsonPropertyName("nhsid_useruid")]
        public string NhsIdUserUid { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("nhsid_nrbac_roles")]
        public List<NhsNrbacRole> NhsIdNrbacRoles { get; set; } = new();

        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;
    }

    public sealed class NhsNrbacRole
    {
        [JsonPropertyName("person_orgid")]
        public string PersonOrgId { get; set; } = string.Empty;

        [JsonPropertyName("person_roleid")]
        public string PersonRoleId { get; set; } = string.Empty;

        [JsonPropertyName("org_code")]
        public string OrgCode { get; set; } = string.Empty;

        [JsonPropertyName("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [JsonPropertyName("role_code")]
        public string RoleCode { get; set; } = string.Empty;

        [JsonPropertyName("activities")]
        public List<string> Activities { get; set; } = new();

        [JsonPropertyName("activity_codes")]
        public List<string> ActivityCodes { get; set; } = new();
    }
}
