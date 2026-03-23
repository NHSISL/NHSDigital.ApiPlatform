// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Text.Json;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Serializations
{
    internal sealed class JsonBroker : IJsonBroker
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

        public T? Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json, Options);

        public string Serialize(object value) =>
            JsonSerializer.Serialize(value, Options);
    }
}
