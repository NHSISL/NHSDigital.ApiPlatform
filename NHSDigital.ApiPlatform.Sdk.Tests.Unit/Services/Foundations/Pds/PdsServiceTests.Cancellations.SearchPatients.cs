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
        public async Task ShouldThrowOperationCanceledExceptionOnSearchPatientsIfCancellationRequestedAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            var cancellationToken = new CancellationToken(canceled: true);

            // when
            ValueTask<string> searchPatientsTask = this.pdsService.SearchPatientsAsync(
                randomAccessToken,
                randomSearchCriteria,
                cancellationToken);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.httpBrokerMock.VerifyNoOtherCalls();
            this.tokenBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotWrapOperationCanceledExceptionRaisedByABrokerOnSearchPatientsAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(new OperationCanceledException(cancellationTokenSource.Token));

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(
                    randomAccessToken,
                    randomSearchCriteria,
                    cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await searchPatientsTask);

            this.loggingBrokerMock.VerifyNoOtherCalls();
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
