// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------
using System;
using System.Threading.Tasks;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.CareIdentityServices.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Foundations.Pds.Exceptions;
using NHSDigital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using Xeptions;

namespace NHSDigital.ApiPlatform.Sdk.Services.Orchestrations.Pds
{
    internal sealed partial class PdsOrchestrationService
    {
        private delegate ValueTask<string> ReturningStringFunction();

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (InvalidArgumentPdsOrchestrationException invalidArgumentPdsOrchestrationException)
            {
                throw await CreateValidationExceptionAsync(invalidArgumentPdsOrchestrationException);
            }
            catch (UnauthorizedPdsOrchestrationException unauthorizedPdsOrchestrationException)
            {
                throw await CreateValidationExceptionAsync(unauthorizedPdsOrchestrationException);
            }
            catch (CareIdentityServiceValidationException careIdentityValidationException)
            {
                throw await CreateDependencyValidationExceptionAsync(careIdentityValidationException);
            }
            catch (CareIdentityServiceDependencyValidationException careIdentityDependencyValidationException)
            {
                throw await CreateDependencyValidationExceptionAsync(careIdentityDependencyValidationException);
            }
            catch (CareIdentityServiceDependencyException careIdentityServiceDependencyException)
            {
                throw await CreateDependencyExceptionAsync(careIdentityServiceDependencyException);
            }
            catch (CareIdentityServiceServiceException careIdentityServiceServiceException)
            {
                throw await CreateDependencyExceptionAsync(careIdentityServiceServiceException);
            }
            catch (PdsServiceValidationException pdsIdentityValidationException)
            {
                throw await CreateDependencyValidationExceptionAsync(pdsIdentityValidationException);
            }
            catch (PdsServiceDependencyValidationException pdsIdentityDependencyValidationException)
            {
                throw await CreateDependencyValidationExceptionAsync(pdsIdentityDependencyValidationException);
            }
            catch (PdsServiceDependencyException pdsIdentityServiceDependencyException)
            {
                throw await CreateDependencyExceptionAsync(pdsIdentityServiceDependencyException);
            }
            catch (PdsServiceException pdsIdentityServiceServiceException)
            {
                throw await CreateDependencyExceptionAsync(pdsIdentityServiceServiceException);
            }
            catch (Exception exception)
            {
                var failedPdsOrchestrationException =
                    new FailedPdsOrchestrationException(
                        message: "Failed PDS orchestration service error occurred, please contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateServiceExceptionAsync(failedPdsOrchestrationException);
            }
        }

        private async ValueTask<PdsOrchestrationValidationException> CreateValidationExceptionAsync(Xeption exception)
        {
            var pdsOrchestrationValidationException =
                new PdsOrchestrationValidationException(
                    message: "PDS orchestration validation error occurred, fix the errors and try again.",
                    innerException: exception);

            return pdsOrchestrationValidationException;
        }

        private async ValueTask<PdsOrchestrationDependencyValidationException> CreateDependencyValidationExceptionAsync(
            Xeption exception)
        {
            var pdsOrchestrationDependencyValidationException =
                new PdsOrchestrationDependencyValidationException(
                    message: "PDS orchestration dependency validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            return pdsOrchestrationDependencyValidationException;
        }

        private async ValueTask<PdsOrchestrationDependencyException> CreateDependencyExceptionAsync(Xeption exception)
        {
            var pdsOrchestrationDependencyException =
                new PdsOrchestrationDependencyException(
                    message: "PDS orchestration dependency error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            return pdsOrchestrationDependencyException;
        }

        private async ValueTask<PdsOrchestrationServiceException> CreateServiceExceptionAsync(Xeption exception)
        {
            var pdsOrchestrationServiceException = new PdsOrchestrationServiceException(
                message: "PDS orchestration service error occurred, please contact support.",
                innerException: exception);

            return pdsOrchestrationServiceException;
        }
    }
}
