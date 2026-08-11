// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Processings.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Processings.CareIdentityServices
{
    internal partial class CareIdentityServiceProcessingService : ICareIdentityServiceProcessingService
    {
        private delegate ValueTask<T> ReturningTaskFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(ReturningTaskFunction<T> returningTaskFunction)
        {
            try
            {
                return await returningTaskFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                throw await CreateAndLogTimeoutDependencyExceptionAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidArgumentCareIdentityServiceProcessingException
                invalidArgumentCareIdentityServiceProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidArgumentCareIdentityServiceProcessingException);
            }
            catch (UnauthorisedCareIdentityServiceProcessingException
                unauthorisedCareIdentityServiceProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    unauthorisedCareIdentityServiceProcessingException);
            }
            catch (CareIdentityServiceProcessingValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingDependencyValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingDependencyException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingServiceException)
            {
                throw;
            }
            catch (CareIdentityServiceValidationException careIdentityServiceValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(careIdentityServiceValidationException);
            }
            catch (CareIdentityServiceDependencyValidationException
                careIdentityServiceDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    careIdentityServiceDependencyValidationException);
            }
            catch (CareIdentityServiceDependencyException careIdentityServiceDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceDependencyException);
            }
            catch (CareIdentityServiceServiceException careIdentityServiceServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceServiceException);
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutCareIdentityServiceProcessingException =
                    new TimeoutCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutExceptionAsync(timeoutCareIdentityServiceProcessingException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceProcessingException =
                    new FailedCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedCareIdentityServiceProcessingException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                throw await CreateAndLogTimeoutDependencyExceptionAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidArgumentCareIdentityServiceProcessingException
                invalidArgumentCareIdentityServiceProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidArgumentCareIdentityServiceProcessingException);
            }
            catch (UnauthorisedCareIdentityServiceProcessingException
                unauthorisedCareIdentityServiceProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    unauthorisedCareIdentityServiceProcessingException);
            }
            catch (CareIdentityServiceProcessingValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingDependencyValidationException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingDependencyException)
            {
                throw;
            }
            catch (CareIdentityServiceProcessingServiceException)
            {
                throw;
            }
            catch (CareIdentityServiceValidationException careIdentityServiceValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(careIdentityServiceValidationException);
            }
            catch (CareIdentityServiceDependencyValidationException
                careIdentityServiceDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    careIdentityServiceDependencyValidationException);
            }
            catch (CareIdentityServiceDependencyException careIdentityServiceDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceDependencyException);
            }
            catch (CareIdentityServiceServiceException careIdentityServiceServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(careIdentityServiceServiceException);
            }
            catch (TimeoutException timeoutException)
            {
                var timeoutCareIdentityServiceProcessingException =
                    new TimeoutCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                throw await CreateAndLogTimeoutExceptionAsync(timeoutCareIdentityServiceProcessingException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceProcessingException =
                    new FailedCareIdentityServiceProcessingException(
                        message: "Failed care identity service processing error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedCareIdentityServiceProcessingException);
            }
        }

        private async ValueTask<CareIdentityServiceProcessingDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync()
        {
            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutCareIdentityServiceProcessingException =
                new TimeoutCareIdentityServiceProcessingException(
                    message: "Failed care identity service processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            return await CreateAndLogTimeoutExceptionAsync(timeoutCareIdentityServiceProcessingException);
        }

        private async ValueTask<CareIdentityServiceProcessingDependencyException> CreateAndLogTimeoutExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceProcessingDependencyException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceProcessingDependencyException);

            return careIdentityServiceProcessingDependencyException;
        }

        private async ValueTask<CareIdentityServiceProcessingValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var careIdentityServiceProcessingValidationException = new CareIdentityServiceProcessingValidationException(
                message: "Care identity service processing validation error occurred, " +
                    "please fix the errors and try again.",

                innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceProcessingValidationException);

            return careIdentityServiceProcessingValidationException;
        }

        private async ValueTask<CareIdentityServiceProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var careIdentityServiceProcessingDependencyValidationException =
                new CareIdentityServiceProcessingDependencyValidationException(
                    message: "Care identity service processing dependency validation error occurred, " +
                        "please fix the errors and try again.",

                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceProcessingDependencyValidationException);

            return careIdentityServiceProcessingDependencyValidationException;
        }

        private async ValueTask<CareIdentityServiceProcessingDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var careIdentityServiceProcessingDependencyException =
                new CareIdentityServiceProcessingDependencyException(
                    message: "Care identity service processing dependency error occurred, please contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceProcessingDependencyException);

            return careIdentityServiceProcessingDependencyException;
        }

        private async ValueTask<CareIdentityServiceProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceProcessingServiceException = new CareIdentityServiceProcessingServiceException(
                message: "Care identity service processing error occurred, please contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(careIdentityServiceProcessingServiceException);

            return careIdentityServiceProcessingServiceException;
        }
    }
}
