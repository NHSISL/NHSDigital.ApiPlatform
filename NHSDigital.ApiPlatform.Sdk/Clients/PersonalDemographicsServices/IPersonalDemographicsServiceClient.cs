// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Patients;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices
{
    public interface IPersonalDemographicsServiceClient
    {
        ValueTask<string> SearchPatientsAsync(
			SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default);
    }
}
