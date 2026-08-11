// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Foundations.Pds
{
    internal partial class PdsService : IPdsService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (NullSearchCriteriaPdsServiceException nullSearchCriteriaPdsServiceException)
            {
                throw await CreateValidationExceptionAsync(nullSearchCriteriaPdsServiceException);
            }
            catch (InvalidArgumentPdsServiceException invalidArgumentPdsServiceException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentPdsServiceException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (TimeoutException timeoutException)
            {
                var failedPdsServiceDependencyException =
                    new FailedPdsServiceDependencyException(
                        message: "Failed PDS service dependency error occurred, please contact support.",
                        innerException: timeoutException);

                throw await CreateDependencyExceptionAsync(failedPdsServiceDependencyException);
            }
            catch (HttpRequestException httpRequestException)
            {
                var failedPdsServiceDependencyException =
                    new FailedPdsServiceDependencyException(
                        message: "Failed PDS service dependency error occurred, please contact support.",
                        innerException: httpRequestException);

                throw await CreateDependencyExceptionAsync(failedPdsServiceDependencyException);
            }
            catch (Exception exception)
            {
                var failedPdsServiceException =
                    new FailedPdsServiceException(
                        message: "Failed PDS service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedPdsServiceException);
            }
        }

        private async ValueTask<PdsServiceValidationException> CreateValidationExceptionAsync(
            Xeption exception)
        {
            var pdsServiceValidationException = new PdsServiceValidationException(
                message: "PDS service validation error occurred, please fix the errors and try again.",
                innerException: exception);

            return pdsServiceValidationException;
        }

        private async ValueTask<PdsServiceDependencyException> CreateDependencyExceptionAsync(Xeption exception)
        {
            var pdsServiceDependencyException = new PdsServiceDependencyException(
                message: "PDS service dependency error occurred, please contact support.",
                innerException: exception);

            return pdsServiceDependencyException;
        }

        private async ValueTask<PdsServiceException> CreateServiceExceptionAsync(Xeption exception)
        {
            var pdsServiceException = new PdsServiceException(
                message: "PDS service error occurred, please contact support.",
                innerException: exception);

            return pdsServiceException;
        }
    }
}
