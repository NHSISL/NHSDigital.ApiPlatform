// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Patients;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds;

namespace NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds
{
    internal sealed partial class PdsOrchestrationService : IPdsOrchestrationService
    {
        private readonly ICareIdentityService careIdentityService;
        private readonly IPdsService pdsService;
        private readonly IApiPlatformTokenBroker tokenBroker;

        public PdsOrchestrationService(ICareIdentityService careIdentityService, IPdsService pdsService, IApiPlatformTokenBroker tokenBroker)
        {
            this.careIdentityService = careIdentityService;
            this.pdsService = pdsService;
            this.tokenBroker = tokenBroker;
        }

        public ValueTask<string> SearchPatientsAsync(
			SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default) =>
            TryCatch(async () =>
            {
                string accessToken = await this.careIdentityService.GetAccessTokenAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new UnauthorizedPdsOrchestrationException("Unauthorized - Unable to retrieve access token.");
                }

                return await this.pdsService
                    .SearchPatientsAsync(accessToken, searchCriteria, cancellationToken);
            });
    }
}
