// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Brokers.Loggings;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    internal partial class CareIdentityServiceProcessingService : ICareIdentityServiceProcessingService
    {
        private readonly ICareIdentityService careIdentityService;
        private readonly ILoggingBroker loggingBroker;

        public CareIdentityServiceProcessingService(
            ICareIdentityService careIdentityService,
            ILoggingBroker loggingBroker)
        {
            this.careIdentityService = careIdentityService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<string> BuildLoginUrlAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.careIdentityService.BuildLoginUrlAsync(cancellationToken);
        });

        public ValueTask LogoutAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await this.careIdentityService.LogoutAsync(cancellationToken);
        });

        public ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.careIdentityService.GetAccessTokenAsync(cancellationToken);
        });

        public ValueTask<NhsUserInfo> GetUserInfoAsync(
            string code,
            string state,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
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
