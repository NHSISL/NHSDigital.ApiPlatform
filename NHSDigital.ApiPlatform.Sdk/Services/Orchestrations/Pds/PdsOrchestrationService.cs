// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Brokers.Loggings;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds;

namespace NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds
{
    internal sealed partial class PdsOrchestrationService : IPdsOrchestrationService
    {
        private readonly ICareIdentityService careIdentityService;
        private readonly IPdsService pdsService;
        private readonly ILoggingBroker loggingBroker;

        public PdsOrchestrationService(
            ICareIdentityService careIdentityService,
            IPdsService pdsService,
            ILoggingBroker loggingBroker)
        {
            this.careIdentityService = careIdentityService;
            this.pdsService = pdsService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<string> SearchPatientsAsync(
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnSearchPatients(searchCriteria);
            string accessToken = await this.careIdentityService.GetAccessTokenAsync(cancellationToken);
            ValidateAccessToken(accessToken);

            return await this.pdsService
                .SearchPatientsAsync(accessToken, searchCriteria, cancellationToken);
        });
    }
}
