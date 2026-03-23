// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;

namespace NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms
{
    internal sealed class ApiPlatformClientFacade : IApiPlatformClient
    {
        public ApiPlatformClientFacade(
            ICareIdentityServiceClient careIdentityServiceClient,
            IPersonalDemographicsServiceClient personalDemographicsServiceClient)
        {
            CareIdentityServiceClient = careIdentityServiceClient;
            PersonalDemographicsServiceClient = personalDemographicsServiceClient;
        }

        public ICareIdentityServiceClient CareIdentityServiceClient { get; }
        public IPersonalDemographicsServiceClient PersonalDemographicsServiceClient { get; }
    }
}
