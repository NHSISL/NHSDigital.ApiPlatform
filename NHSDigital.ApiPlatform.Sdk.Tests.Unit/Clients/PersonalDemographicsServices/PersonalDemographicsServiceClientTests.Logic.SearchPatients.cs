// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        [Fact]
        public async Task ShouldSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            string randomPayload = GetRandomString();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(randomSearchCriteria, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomPayload);

            // when
            string actualPayload =
                await this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            // then
            actualPayload.Should().Be(randomPayload);

            this.pdsOrchestrationServiceMock.Verify(service =>
                service.SearchPatientsAsync(randomSearchCriteria, It.IsAny<CancellationToken>()),
                    Times.Once);
        }
    }
}
