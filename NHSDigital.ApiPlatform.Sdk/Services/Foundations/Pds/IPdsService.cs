// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Patients;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal interface IPdsService
    {
        ValueTask<string> SearchPatientsAsync(
            string accessToken,
			SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default);
    }
}
