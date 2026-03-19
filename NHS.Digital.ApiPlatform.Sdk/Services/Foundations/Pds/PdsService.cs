// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NHS.Digital.ApiPlatform.Sdk.Brokers.Https;
using NHS.Digital.ApiPlatform.Sdk.Brokers.Identifiers;
using NHS.Digital.ApiPlatform.Sdk.Models.Configurations;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Pds;

namespace NHS.Digital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private readonly ApiPlatformConfigurations configurations;
        private readonly IHttpBroker httpBroker;
        private readonly IIdentifierBroker identifierBroker;

        public PdsService(
            ApiPlatformConfigurations configurations,
            IHttpBroker httpBroker,
            IIdentifierBroker identifierBroker)
        {
            this.configurations = configurations;
            this.httpBroker = httpBroker;
            this.identifierBroker = identifierBroker;
        }

		public ValueTask<string> SearchPatientsAsync(
			string accessToken,
			SearchCriteria searchCriteria,
			CancellationToken cancellationToken = default) =>
		TryCatch(async () =>
		{
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
					url += $"&birthdate=eq{searchCriteria.DateOfBirth}";
				}

				if (!string.IsNullOrWhiteSpace(searchCriteria.Postcode))
				{
					url += $"&postcode={Uri.EscapeDataString(searchCriteria.Postcode)}";
				}
			}

			var response = await this.httpBroker.GetAsync(
				url,
				request =>
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
					request.Headers.Add("X-Request-ID", this.identifierBroker.GetNewGuid().ToString());
					request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
				},
				cancellationToken);

			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync(cancellationToken);
		});
	}
}
