using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EAVStore.Api.ViewModels;

namespace EAVStore.Api.Services
{
    public interface IClinicService
    {
        Task AddPatientAsync(CreatePatientVm patientDto);
        Task<IReadOnlyCollection<PatientVm>> GetAllPatientsAsync(CancellationToken cancellationToken);
        Task<PatientVm> GetPatientAsync(Guid patientId, CancellationToken cancellationToken);
        Task AddOperationAsync(CreateOperationVm operationDto);
    }
}