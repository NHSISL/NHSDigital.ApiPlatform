// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Patients;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace NHS.Digital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal interface IPdsService
    {
        ValueTask<string> SearchPatientsAsync(
            string accessToken,
			SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default);
    }
}
