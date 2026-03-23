// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using NHSDigital.ApiPlatform.Sdk.Clients.CareIdentityServices;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;

namespace NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms
{
    public interface IApiPlatformClient
    {
        ICareIdentityServiceClient CareIdentityServiceClient { get; }
        IPersonalDemographicsServiceClient PersonalDemographicsServiceClient { get; }
    }
}
