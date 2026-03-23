// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Identifiers
{
    public interface IIdentifierBroker
    {
        Guid GetNewGuid();
    }
}
