// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NHSDigital.ApiPlatform.Sdk.Brokers.Https
{
    public interface IHttpBroker
    {
        ValueTask<HttpResponseMessage> PostFormAsync(
            string url,
            IEnumerable<KeyValuePair<string, string>> formValues,
            CancellationToken cancellationToken);

        ValueTask<HttpResponseMessage> GetAsync(
            string url,
            Action<HttpRequestMessage>? configureRequest,
            CancellationToken cancellationToken);
    }
}
