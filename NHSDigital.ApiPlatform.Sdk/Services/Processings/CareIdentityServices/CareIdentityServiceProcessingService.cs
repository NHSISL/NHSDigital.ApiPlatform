// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    internal partial class CareIdentityServiceProcessingService : ICareIdentityServiceProcessingService
    {
        private readonly ICareIdentityService careIdentityService;

        public CareIdentityServiceProcessingService(ICareIdentityService careIdentityService) =>
            this.careIdentityService = careIdentityService;

        public ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            return await this.careIdentityService.BuildLoginUrlAsync(cancellationToken);
        });


        public ValueTask LogoutAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.careIdentityService.LogoutAsync(cancellationToken);
        });

        public ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            return await this.careIdentityService.GetAccessTokenAsync(cancellationToken);
        });

        public ValueTask<NhsUserInfo> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateOnGetUserInfo(code, state);
            await this.careIdentityService.CallbackAsync(code, state, cancellationToken);
            string accessToken = await this.careIdentityService.GetAccessTokenAsync(cancellationToken);
            ValidateAccessToken(accessToken);

            NhsUserInfo userInfo = await this.careIdentityService
                .GetUserInfoAsync(accessToken, cancellationToken);

            return userInfo;
        });

    }
}
