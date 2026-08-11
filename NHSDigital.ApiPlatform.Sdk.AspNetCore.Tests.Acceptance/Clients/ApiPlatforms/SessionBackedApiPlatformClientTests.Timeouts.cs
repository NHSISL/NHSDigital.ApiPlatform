// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.AspNetCore.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class SessionBackedApiPlatformClientTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfTheTokenEndpointTimesOutAsync()
        {
            // given
            this.wireMockServer
                .Given(Request.Create().WithPath(TokenPath).UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithDelay(TimeSpan.FromSeconds(10))
                    .WithBody("{}"));

            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(async () =>
                    await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();

            actualException.InnerException.InnerException.Message
                .Should().Be("The dependency operation timed out.");
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfThePatientEndpointTimesOutAsync()
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            await GivenAnAuthenticatedSessionAsync(GetRandomString(), GetRandomString());

            this.wireMockServer
                .Given(Request.Create().WithPath($"{FhirPath}/Patient/{randomNhsNumber}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithDelay(TimeSpan.FromSeconds(10))
                    .WithBody("{}"));

            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(async () =>
                    await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<TimeoutException>();
        }
    }
}
