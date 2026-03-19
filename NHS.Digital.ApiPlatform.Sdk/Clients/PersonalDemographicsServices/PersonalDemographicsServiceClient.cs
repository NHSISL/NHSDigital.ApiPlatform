// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHS.Digital.ApiPlatform.Sdk.Models.Clients.Pds.Exceptions;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Patients;
using NHS.Digital.ApiPlatform.Sdk.Models.Foundations.Pds;
using NHS.Digital.ApiPlatform.Sdk.Models.Orchestrations.Pds.Exceptions;
using NHS.Digital.ApiPlatform.Sdk.Services.Orchestrations.Pds;
using Xeptions;

namespace NHS.Digital.ApiPlatform.Sdk.Clients.PersonalDemographicsServices
{
    internal class PersonalDemographicsServiceClient : IPersonalDemographicsServiceClient
    {
        private readonly IPdsOrchestrationService pdsOrchestrationService;

        public PersonalDemographicsServiceClient(IPdsOrchestrationService pdsOrchestrationService) =>
            this.pdsOrchestrationService = pdsOrchestrationService;

        public async ValueTask<string> SearchPatientsAsync(
			SearchCriteria searchCriteria,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.pdsOrchestrationService.SearchPatientsAsync(
					searchCriteria,
                    cancellationToken);
            }
            catch (PdsOrchestrationValidationException pdsOrchestrationValidationException)
            {
                throw CreatePersonalDemographicsServiceClientValidationException(
                    pdsOrchestrationValidationException.InnerException as Xeption);
            }
            catch (PdsOrchestrationDependencyValidationException
                pdsOrchestrationDependencyValidationException)
            {
                throw CreatePersonalDemographicsServiceClientValidationException(
                    pdsOrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (PdsOrchestrationDependencyException pdsOrchestrationDependencyException)
            {
                throw CreatePersonalDemographicsServiceClientDependencyException(
                    pdsOrchestrationDependencyException.InnerException as Xeption);
            }
            catch (PdsOrchestrationServiceException pdsOrchestrationServiceException)
            {
                throw CreatePersonalDemographicsServiceClientServiceException(
                    pdsOrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreatePersonalDemographicsServiceClientServiceException(
                    new FailedPersonalDemographicsServiceClientException(
                        message: "Unexpected error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data));
            }
        }

        private static PersonalDemographicsServiceClientValidationException
            CreatePersonalDemographicsServiceClientValidationException(Xeption innerException)
        {
            return new PersonalDemographicsServiceClientValidationException(
                message: "Personal demographics service client validation error occurred, fix errors and try again.",
                innerException);
        }

        private static PersonalDemographicsServiceClientDependencyException
            CreatePersonalDemographicsServiceClientDependencyException(Xeption innerException)
        {
            return new PersonalDemographicsServiceClientDependencyException(
                message: "Personal demographics service client dependency error occurred, contact support.",
                innerException);
        }

        private static PersonalDemographicsServiceClientServiceException
            CreatePersonalDemographicsServiceClientServiceException(Xeption innerException)
        {
            return new PersonalDemographicsServiceClientServiceException(
                message: "Personal demographics service client service error occurred, contact support.",
                innerException);
        }
    }
}
