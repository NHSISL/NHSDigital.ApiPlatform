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
    internal sealed class HttpBroker : IHttpBroker
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpBroker(IHttpClientFactory httpClientFactory) =>
            this.httpClientFactory = httpClientFactory;

        public async ValueTask<HttpResponseMessage> PostFormAsync(
            string url,
            IEnumerable<KeyValuePair<string, string>> formValues,
            CancellationToken cancellationToken)
        {
            HttpClient client = this.httpClientFactory.CreateClient("NhsApiPlatform");
            var content = new FormUrlEncodedContent(formValues);

            return await client.PostAsync(url, content, cancellationToken);
        }

        public async ValueTask<HttpResponseMessage> GetAsync(
            string url,
            Action<HttpRequestMessage>? configureRequest,
            CancellationToken cancellationToken)
        {
            HttpClient client = this.httpClientFactory.CreateClient("NhsApiPlatform");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            configureRequest?.Invoke(request);

            return await client.SendAsync(request, cancellationToken);
        }
    }
}
