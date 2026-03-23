// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Serializations
{
    public interface IJsonBroker
    {
        T? Deserialize<T>(string json);
        string Serialize(object value);
    }
}
