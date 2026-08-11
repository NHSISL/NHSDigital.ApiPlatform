// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Clients.PersonalDemographicsServices
{
    public partial class PersonalDemographicsServiceClientTests
    {
        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldNotWrapTaskCanceledExceptionOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new TaskCanceledException());

            // when
            ValueTask<string> searchPatientsTask =
                this.personalDemographicsServiceClient.SearchPatientsAsync(randomSearchCriteria);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToOrchestrationServiceOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.pdsOrchestrationServiceMock.Setup(service =>
                service.SearchPatientsAsync(It.IsAny<SearchCriteria>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetRandomString());

            // when
            await this.personalDemographicsServiceClient.SearchPatientsAsync(
                randomSearchCriteria,
                cancellationToken);

            // then
            this.pdsOrchestrationServiceMock.Verify(service =>
                service.SearchPatientsAsync(randomSearchCriteria, cancellationToken),
                    Times.Once);
        }
    }
}
