// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.CareIdentityService.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Acceptance.Clients.ApiPlatforms
{
    public partial class ApiPlatformClientTests
    {
        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task ShouldThrowDependencyExceptionOnGetUserInfoIfTokenEndpointFailsAsync(
            HttpStatusCode statusCode)
        {
            // given
            GivenTokenEndpointFailsWith(statusCode);
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            CareIdentityServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyException>(async () =>
                    await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnGetUserInfoIfStateDoesNotMatchAsync()
        {
            // given
            GivenTokenEndpointReturns(GetRandomString(), GetRandomString());
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string tamperedState = GetRandomString();

            // when
            CareIdentityServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyValidationException>(async () =>
                    await this.careIdentityServiceClient.GetUserInfoAsync(
                        GetRandomString(),
                        tamperedState));

            // then
            actualException.InnerException.Message.Should().Be("Invalid state parameter.");
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfPdsFailsAsync(
            HttpStatusCode statusCode)
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            await GivenAnAuthenticatedSessionAsync();
            GivenPatientEndpointFailsWith(randomNhsNumber, statusCode);
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            PersonalDemographicsServiceClientDependencyException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyException>(async () =>
                    await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task ShouldThrowDependencyValidationExceptionOnSearchPatientsIfPdsRejectsTheRequestAsync(
            HttpStatusCode statusCode)
        {
            // given
            string randomNhsNumber = GetRandomNhsNumber();
            await GivenAnAuthenticatedSessionAsync();
            GivenPatientEndpointFailsWith(randomNhsNumber, statusCode);
            SearchCriteria searchCriteria = CreateSearchCriteriaByNhsNumber(randomNhsNumber);

            // when
            PersonalDemographicsServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<PersonalDemographicsServiceClientDependencyValidationException>(
                    async () => await this.personalDemographicsServiceClient.SearchPatientsAsync(searchCriteria));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task ShouldThrowDependencyValidationExceptionOnGetUserInfoIfTheTokenEndpointRejectsUsAsync(
            HttpStatusCode statusCode)
        {
            // given
            GivenTokenEndpointFailsWith(statusCode);
            GivenUserInfoEndpointReturns(GetRandomString(), GetRandomString());
            string loginUrl = await this.careIdentityServiceClient.BuildLoginUrlAsync();
            string state = ExtractStateFromLoginUrl(loginUrl);

            // when
            CareIdentityServiceClientDependencyValidationException actualException =
                await Assert.ThrowsAsync<CareIdentityServiceClientDependencyValidationException>(async () =>
                    await this.careIdentityServiceClient.GetUserInfoAsync(GetRandomString(), state));

            // then
            actualException.InnerException.InnerException.Should().BeOfType<HttpRequestException>();
        }
    }
}
