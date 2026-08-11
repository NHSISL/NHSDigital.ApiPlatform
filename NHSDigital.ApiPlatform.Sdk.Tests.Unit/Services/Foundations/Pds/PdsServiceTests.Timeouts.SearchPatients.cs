// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xunit;

namespace NHSDigital.ApiPlatform.Sdk.Tests.Unit.Services.Foundations.Pds
{
    public partial class PdsServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfOperationCanceledExceptionOccursAndLogItAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            var operationCanceledException = new OperationCanceledException();

            PdsServiceDependencyException expectedPdsServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(operationCanceledException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceDependencyException actualPdsServiceDependencyException =
                await Assert.ThrowsAsync<PdsServiceDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceDependencyException
                .Should().BeEquivalentTo(expectedPdsServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedPdsServiceDependencyException))),
                        Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfTaskCanceledExceptionOccursAndLogItAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();
            var taskCanceledException = new TaskCanceledException();

            PdsServiceDependencyException expectedPdsServiceDependencyException =
                CreateExpectedTimeoutDependencyException();

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(taskCanceledException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceDependencyException actualPdsServiceDependencyException =
                await Assert.ThrowsAsync<PdsServiceDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceDependencyException
                .Should().BeEquivalentTo(expectedPdsServiceDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedPdsServiceDependencyException))),
                        Times.Once);
        }

        private static PdsServiceDependencyException CreateExpectedTimeoutDependencyException()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutPdsServiceException =
                new TimeoutPdsServiceException(
                    message: "Failed PDS service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return new PdsServiceDependencyException(
                message: "PDS service dependency error occurred, please contact support.",
                innerException: timeoutPdsServiceException);
        }
    }
}
