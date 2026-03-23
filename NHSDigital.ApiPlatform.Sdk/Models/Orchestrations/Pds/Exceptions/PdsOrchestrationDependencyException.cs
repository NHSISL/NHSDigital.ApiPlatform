// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions
{
    public class PdsOrchestrationDependencyException : Xeption
    {
        public PdsOrchestrationDependencyException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
