// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
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
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedPatientServiceException);
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
            catch (Exception exception)
            {
                var failedPatientServiceException =
                    new FailedCareIdentityServiceException(
                        message: "Failed care identity service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedPatientServiceException);
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
