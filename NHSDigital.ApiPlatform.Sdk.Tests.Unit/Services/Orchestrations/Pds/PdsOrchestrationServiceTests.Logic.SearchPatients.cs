// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldReturnPatientPayloadOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            string randomPayload = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.pdsServiceMock.Setup(service =>
                service.SearchPatientsAsync(
                    randomAccessToken,
                    randomSearchCriteria,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(randomPayload);

            // when
            string actualPayload =
                await this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            // then
            actualPayload.Should().Be(randomPayload);
        }

        [Fact]
        public async Task ShouldRetrieveAccessTokenBeforeSearchingPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomAccessToken);

            this.pdsServiceMock.Setup(service =>
                service.SearchPatientsAsync(
                    It.IsAny<string>(),
                    It.IsAny<SearchCriteria>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(GetRandomString());

            // when
            await this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.SearchPatientsAsync(
                    randomAccessToken,
                    randomSearchCriteria,
                    It.IsAny<CancellationToken>()),
                        Times.Once);
        }
    }
}
