// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Clients.ApiPlatforms;
using NHSDigital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Configurations;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Integration.Clients.PersonalDemographicsServices
{
    public class PersonalDemographicsServiceClientTests
    {
        private readonly ApiPlatformConfigurations apiPlatformConfigurations;
        private readonly IPersonalDemographicsServiceClient personalDemographicsServiceClient;

        public PersonalDemographicsServiceClientTests()
        {
            this.apiPlatformConfigurations = ConfigurationProvider.GetApiPlatformConfigurations();
            var apiPlatformClient = new ApiPlatformClient(this.apiPlatformConfigurations);
            this.personalDemographicsServiceClient = apiPlatformClient.PersonalDemographicsServiceClient;
        }

        [Fact]
        public void ShouldResolveThePersonalDemographicsServiceBaseUrlFromConfiguration()
        {
            // given
            // when
            string actualBaseUrl = this.apiPlatformConfigurations.PersonalDemographicsService.BaseUrl;

            // then
            actualBaseUrl.Should().NotBeNullOrWhiteSpace(
                "appsettings.json must supply the PDS FHIR base url");
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfSearchCriteriaIsNullAsync()
        {
            // given
            SearchCriteria nullSearchCriteria = null;

            // when
            // then
            await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(nullSearchCriteria));
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSearchPatientsIfNotAuthenticatedAsync()
        {
            // given
            var searchCriteria = new SearchCriteria { NhsNumber = GetRandomNhsNumber() };

            // when
            PersonalDemographicsServiceClientValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientValidationException>(async () =>
                    await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.Message
                .Should().Be("Unauthorized - Unable to retrieve access token.");
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfTokenIsAlreadyCancelledAsync()
        {
            // given
            var searchCriteria = new SearchCriteria { NhsNumber = GetRandomNhsNumber() };
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
                await this.personalDemographicsServiceClient.SearchPatientsAsync(
                    searchCriteria,
                    cancellationTokenSource.Token));
        }

        [Fact(Skip = "Requires NHS CIS2 credentials and reaches the live INT PDS endpoint.")]
        public async Task ShouldSearchPatientsByNhsNumberAsync()
        {
            // given
            var searchCriteria = new SearchCriteria { NhsNumber = "9000000009" };

            // when
            string actualPayload =
                await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria);

            // then
            actualPayload.Should().Contain("Patient");
        }

        private static string GetRandomNhsNumber() =>
            new IntRange(min: 100000000, max: 999999999).GetValue().ToString();
    }
}
