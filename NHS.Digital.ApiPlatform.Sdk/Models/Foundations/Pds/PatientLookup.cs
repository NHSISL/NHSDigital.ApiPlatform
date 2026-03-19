// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Patients;

namespace NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Pds
{
    public class PatientLookup
    {
        public SearchCriteria SearchCriteria { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
