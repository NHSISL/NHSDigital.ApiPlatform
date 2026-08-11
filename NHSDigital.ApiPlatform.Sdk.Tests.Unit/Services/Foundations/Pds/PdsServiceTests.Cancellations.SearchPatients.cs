// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfTokenIsAlreadyCancelledAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask<string> searchPatientsTask = this.pdsService.SearchPatientsAsync(
                randomAccessToken,
                randomSearchCriteria,
                cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(new OperationCanceledException());

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldNotWrapTaskCanceledExceptionOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(new TaskCanceledException());

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            // then
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await searchPatientsTask);
        }

        [Fact]
        public async Task ShouldPropagateCancellationTokenToBrokersOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(CreateHttpResponse(GetRandomString()));

            // when
            await this.pdsService.SearchPatientsAsync(
                randomAccessToken,
                randomSearchCriteria,
                cancellationToken);

            // then
            this.httpBrokerMock.Verify(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    cancellationToken),
                        Times.Once);

            this.tokenBrokerMock.Verify(broker =>
                broker.GetActiveRoleAsync(cancellationToken),
                    Times.Once);
        }
    }
}
