// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Cryptographies
{
    public interface ICryptoBroker
    {
        string CreateUrlSafeState(int bytes = 32);
    }
}
