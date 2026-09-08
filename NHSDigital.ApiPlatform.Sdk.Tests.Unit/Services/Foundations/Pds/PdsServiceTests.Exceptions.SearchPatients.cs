// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
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
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfDependencyErrorOccursAsync(
            Exception dependencyException)
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            PdsServiceDependencyException expectedPdsServiceDependencyException =
                CreateExpectedDependencyException(dependencyException);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(dependencyException);

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
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsServiceDependencyException))),
                    Times.Once);
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowServiceExceptionOnSearchPatientsIfServiceErrorOccursAsync(
            Exception serviceException)
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            PdsServiceException expectedPdsServiceException =
                CreateExpectedServiceException(serviceException);

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .Throws(serviceException);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceException actualPdsServiceException =
                await Assert.ThrowsAsync<PdsServiceException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceException.Should().BeEquivalentTo(expectedPdsServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedPdsServiceException))),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSearchPatientsIfResponseIsUnsuccessfulAsync()
        {
            // given
            string randomAccessToken = GetRandomString();
            SearchCriteria randomSearchCriteria = CreateRandomSearchCriteriaWithNhsNumber();

            var unsuccessfulResponse =
                new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(GetRandomString())
                };

            this.httpBrokerMock.Setup(broker =>
                broker.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Action<HttpRequestMessage>>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(unsuccessfulResponse);

            // when
            ValueTask<string> searchPatientsTask =
                this.pdsService.SearchPatientsAsync(randomAccessToken, randomSearchCriteria);

            PdsServiceDependencyException actualPdsServiceDependencyException =
                await Assert.ThrowsAsync<PdsServiceDependencyException>(
                    async () => await searchPatientsTask);

            // then
            actualPdsServiceDependencyException.InnerException.InnerException
                .Should().BeOfType<HttpRequestException>();
        }
    }
}
