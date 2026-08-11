// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(
            ReturningStringFunction returningStringFunction,
            CancellationToken cancellationToken)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested is false)
            {
                throw await CreateAndLogTimeoutDependencyExceptionAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullSearchCriteriaPdsServiceException nullSearchCriteriaPdsServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullSearchCriteriaPdsServiceException);
            }
            catch (InvalidArgumentPdsServiceException invalidArgumentPdsServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentPdsServiceException);
            }
            catch (PdsServiceValidationException)
            {
                throw;
            }
            catch (PdsServiceDependencyValidationException)
            {
                throw;
            }
            catch (PdsServiceDependencyException)
            {
                throw;
            }
            catch (PdsServiceException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutPdsServiceException =
                    new TimeoutPdsServiceException(
                        message: "Failed PDS service timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutPdsServiceException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedPdsServiceDependencyException =
                    new FailedPdsServiceDependencyException(
                        message: "Failed PDS service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateAndLogDependencyExceptionAsync(failedPdsServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedPdsServiceException =
                    new FailedPdsServiceException(
                        message: "Failed PDS service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedPdsServiceException);
            }
        }

        private async ValueTask<PdsServiceDependencyException> CreateAndLogTimeoutDependencyExceptionAsync()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutPdsServiceException =
                new TimeoutPdsServiceException(
                    message: "Failed PDS service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return await CreateAndLogDependencyExceptionAsync(timeoutPdsServiceException);
        }

        private async ValueTask<PdsServiceValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var pdsServiceValidationException = new PdsServiceValidationException(
                message: "PDS service validation error occurred, please fix the errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsServiceValidationException);

            return pdsServiceValidationException;
        }

        private async ValueTask<PdsServiceDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var pdsServiceDependencyException = new PdsServiceDependencyException(
                message: "PDS service dependency error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsServiceDependencyException);

            return pdsServiceDependencyException;
        }

        private async ValueTask<PdsServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var pdsServiceException = new PdsServiceException(
                message: "PDS service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(pdsServiceException);

            return pdsServiceException;
        }
    }
}
