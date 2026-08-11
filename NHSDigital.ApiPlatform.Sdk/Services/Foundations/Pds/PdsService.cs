// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Brokers.Https;
using NHSDigital.ApiPlatform.Sdk.Brokers.Identifiers;
using NHSDigital.ApiPlatform.Sdk.Brokers.Loggings;
using NHSDigital.ApiPlatform.Sdk.Brokers.Storages;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private readonly ApiPlatformConfigurations configurations;
        private readonly IHttpBroker httpBroker;
        private readonly IIdentifierBroker identifierBroker;
        private readonly IApiPlatformTokenBroker tokenBroker;
        private readonly ILoggingBroker loggingBroker;

        public PdsService(
            ApiPlatformConfigurations configurations,
            IHttpBroker httpBroker,
            IIdentifierBroker identifierBroker,
            IApiPlatformTokenBroker tokenBroker,
            ILoggingBroker loggingBroker)
        {
            this.loggingBroker = loggingBroker;
            this.configurations = configurations;
            this.httpBroker = httpBroker;
            this.identifierBroker = identifierBroker;
            this.tokenBroker = tokenBroker;
        }

        public ValueTask<string> SearchPatientsAsync(
            string accessToken,
            SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnSearchPatients(accessToken, searchCriteria);
            string baseUrl = this.configurations.PersonalDemographicsService.BaseUrl.TrimEnd('/');
            string url;

            if (!string.IsNullOrWhiteSpace(searchCriteria.NhsNumber))
            {
                url = $"{baseUrl}/Patient/{searchCriteria.NhsNumber}";
            }
            else
            {
                url = $"{baseUrl}/Patient?family={Uri.EscapeDataString(searchCriteria.Surname)}";

                if (!string.IsNullOrWhiteSpace(searchCriteria.FirstName))
                {
                    url += $"&given={Uri.EscapeDataString(searchCriteria.FirstName)}";
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.Gender))
                {
                    url += $"&gender={Uri.EscapeDataString(searchCriteria.Gender)}";
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.DateOfBirth))
                {
                    url += $"&birthdate=eq{searchCriteria.DateOfBirth:yyyy-MM-dd}";
                }

                if (!string.IsNullOrWhiteSpace(searchCriteria.Postcode))
                {
                    url += $"&address-postalcode={Uri.EscapeDataString(searchCriteria.Postcode)}";
                }
            }

            string activeRoleId = await this.tokenBroker.GetActiveRoleAsync(cancellationToken);

            var response = await this.httpBroker.GetAsync(
                url,
                request =>
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.Add("X-Request-ID", this.identifierBroker.GetNewGuid().ToString());
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

                    if (!string.IsNullOrWhiteSpace(activeRoleId))
                    {
                        request.Headers.Add("NHSD-Session-URID", activeRoleId);
                    }
                },
                cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        });
    }
}
