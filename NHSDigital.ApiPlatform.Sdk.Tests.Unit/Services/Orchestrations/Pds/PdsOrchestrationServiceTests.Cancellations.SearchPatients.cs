// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Orchestrations.Pds
{
    public partial class PdsOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfTokenIsAlreadyCancelledAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<string> searchPatientsTask = this.pdsOrchestrationService.SearchPatientsAsync(
                randomSearchCriteria,
                cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();

            this.careIdentityServiceMock.Setup(service =>
                service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new OperationCanceledException());

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToDependenciesOnSearchPatientsAsync()
        {
            // given
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteria();
            string randomAccessToken = GetRandomString();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

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
            await this.pdsOrchestrationService.SearchPatientsAsync(randomSearchCriteria, cancellationToken);

            // then
            this.careIdentityServiceMock.Verify(service =>
                service.GetAccessTokenAsync(cancellationToken),
                    Times.Once);

            this.pdsServiceMock.Verify(service =>
                service.SearchPatientsAsync(randomAccessToken, randomSearchCriteria, cancellationToken),
                    Times.Once);
        }
    }
}
