// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    public interface ICareIdentityServiceProcessingService
    {
        ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default);
        ValueTask LogoutAsync(CancellationToken cancellationToken = default);
        ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        ValueTask<NhsUserInfo> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default);
    }
}
