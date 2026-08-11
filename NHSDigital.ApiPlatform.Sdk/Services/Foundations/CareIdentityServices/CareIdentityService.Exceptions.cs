// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.CareIdentityServices
{
    internal partial class CareIdentityService
    {
        private delegate ValueTask<T> ReturningTaskFunction<T>();
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask<T> TryCatch<T>(ReturningTaskFunction<T> returningTaskFunction)
        {
            try
            {
                return await returningTaskFunction();
            }
            catch (InvalidArgumentCareIdentityServiceException invalidArgumentCareIdentityServiceException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentCareIdentityServiceException);
            }
            catch (UnauthorisedCareIdentityServiceException unauthorisedCareIdentityServiceException)
            {
                throw await CreateValidationExceptionAsync(unauthorisedCareIdentityServiceException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: timeoutException);

                throw await CreateDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedCareIdentityServiceException);
            }
        }

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidArgumentCareIdentityServiceException invalidArgumentCareIdentityServiceException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentCareIdentityServiceException);
            }
            catch (UnauthorisedCareIdentityServiceException unauthorisedCareIdentityServiceException)
            {
                throw await CreateValidationExceptionAsync(unauthorisedCareIdentityServiceException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: timeoutException);

                throw await CreateDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedCareIdentityServiceDependencyException =
                    new FailedCareIdentityServiceDependencyException(
                        message: "Failed care identity service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateDependencyExceptionAsync(failedCareIdentityServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedCareIdentityServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedCareIdentityServiceException);
            }
        }

        private async ValueTask<CareIdentityServiceValidationException> CreateValidationExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceValidationException = new CareIdentityServiceValidationException(
                message: "Care identity service validation error occurred, please fix the errors and try again.",
                innerException: exception);

            return careIdentityServiceValidationException;
        }

        private async ValueTask<CareIdentityServiceDependencyException> CreateDependencyExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceDependencyException = new CareIdentityServiceDependencyException(
                message: "Care identity service dependency error occurred, please contact support.",
                innerException: exception);

            return careIdentityServiceDependencyException;
        }

        private async ValueTask<CareIdentityServiceServiceException> CreateServiceExceptionAsync(
            Xeption exception)
        {
            var careIdentityServiceServiceException = new CareIdentityServiceServiceException(
                message: "Care identity service error occurred, please contact support.",
                innerException: exception);

            return careIdentityServiceServiceException;
        }
    }
}
