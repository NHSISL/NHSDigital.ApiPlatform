// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Patients;

namespace NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds
{
    public class PatientLookup
    {
        public SearchCriteria SearchCriteria { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
