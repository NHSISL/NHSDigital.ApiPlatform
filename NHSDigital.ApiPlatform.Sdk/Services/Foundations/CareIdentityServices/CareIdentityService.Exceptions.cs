// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices
{
    internal partial class CareIdentityService
    {
        private delegate ValueTask<T> ReturningTaskFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(
            ReturningTaskFunction<T> returningTaskFunction,
            CancellationToken cancellationToken)
        {
            try
            {
                return await returningTaskFunction();
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
            catch (InvalidArgumentCareIdentityServiceException invalidArgumentCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentCareIdentityServiceException);
            }
            catch (UnauthorisedCareIdentityServiceException unauthorisedCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorisedCareIdentityServiceException);
            }
            catch (InvalidStateCareIdentityServiceException invalidStateCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidStateCareIdentityServiceException);
            }
            catch (CareIdentityServiceValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceDependencyValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceDependencyException)
            {
                throw;
            }
            catch (CareIdentityServiceServiceException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutCareIdentityServiceException =
                    new TimeoutCareIdentityServiceException(
                        message: "Failed care identity service timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutCareIdentityServiceException);
            }
            catch (HttpRequestException httpRequestException)
                when (IsDependencyValidationStatusCode(httpRequestException.StatusCode))
            {
                var invalidCareIdentityServiceDependencyException =
                    new InvalidCareIdentityServiceDependencyException(
                        message: "Invalid care identity service dependency error occurred, " +
                            "fix the errors and try again.",

                        innerException: httpRequestException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidCareIdentityServiceDependencyException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateAndLogDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedCareIdentityServiceException);
            }
        }

        private async ValueTask TryCatch(
            ReturningNothingFunction returningNothingFunction,
            CancellationToken cancellationToken)
        {
            try
            {
                await returningNothingFunction();
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
            catch (InvalidArgumentCareIdentityServiceException invalidArgumentCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArgumentCareIdentityServiceException);
            }
            catch (UnauthorisedCareIdentityServiceException unauthorisedCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(unauthorisedCareIdentityServiceException);
            }
            catch (InvalidStateCareIdentityServiceException invalidStateCareIdentityServiceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidStateCareIdentityServiceException);
            }
            catch (CareIdentityServiceValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceDependencyValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceDependencyException)
            {
                throw;
            }
            catch (CareIdentityServiceServiceException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutCareIdentityServiceException =
                    new TimeoutCareIdentityServiceException(
                        message: "Failed care identity service timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogDependencyExceptionAsync(timeoutCareIdentityServiceException);
            }
            catch (HttpRequestException httpRequestException)
                when (IsDependencyValidationStatusCode(httpRequestException.StatusCode))
            {
                var invalidCareIdentityServiceDependencyException =
                    new InvalidCareIdentityServiceDependencyException(
                        message: "Invalid care identity service dependency error occurred, " +
                            "fix the errors and try again.",

                        innerException: httpRequestException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    invalidCareIdentityServiceDependencyException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateAndLogDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedCareIdentityServiceException);
            }
        }

        private async ValueTask<CareIdentityServiceDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceException =
                new TimeoutCareIdentityServiceException(
                    message: "Failed care identity service timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return await CreateAndLogDependencyExceptionAsync(timeoutCareIdentityServiceException);
        }

        private async ValueTask<CareIdentityServiceValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceValidationException = new CareIdentityServiceValidationException(
                message: "Care identity service validation error occurred, please fix the errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceValidationException);

            return careIdentityServiceValidationException;
        }

        // A 4xx tells us the dependency rejected what we sent it, which the caller can act on.
        // A 5xx (or a transport failure, where StatusCode is null) is not the caller's to fix.
        private static bool IsDependencyValidationStatusCode(HttpStatusCode? statusCode) =>
            statusCode >= HttpStatusCode.BadRequest && statusCode < HttpStatusCode.InternalServerError;

        private async ValueTask<CareIdentityServiceDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var careIdentityServiceDependencyValidationException =
                new CareIdentityServiceDependencyValidationException(
                    message: "Care identity service dependency validation error occurred, " +
                        "fix the errors and try again.",

                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceDependencyValidationException);

            return careIdentityServiceDependencyValidationException;
        }

        private async ValueTask<CareIdentityServiceDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceDependencyException = new CareIdentityServiceDependencyException(
                message: "Care identity service dependency error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceDependencyException);

            return careIdentityServiceDependencyException;
        }

        private async ValueTask<CareIdentityServiceServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceServiceException = new CareIdentityServiceServiceException(
                message: "Care identity service error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceServiceException);

            return careIdentityServiceServiceException;
        }
    }
}
