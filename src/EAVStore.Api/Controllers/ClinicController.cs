using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EAVStore.Api.Services;
using EAVStore.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EAVStore.Api.Controllers
{
    [Route("clinic")]
    [ApiController]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService) {
            _clinicService = clinicService ?? throw new ArgumentNullException(nameof(clinicService));
        }

        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<PatientVm>>> GetAllPatientsAsync(CancellationToken cancellationToken) {
            return Ok(
                await _clinicService.GetAllPatientsAsync(cancellationToken)
            );
        }

        [HttpGet("patients/{patientId}")]
        public async Task<ActionResult<PatientVm>> GetPatientAsync(Guid patientId, CancellationToken cancellationToken) {
            return Ok(
                await _clinicService.GetPatientAsync(patientId, cancellationToken)
            );
        }

        [HttpPost("patients/new")]
        public async Task<IActionResult> CreateNewPatient(CreatePatientVm createPatientVm) {
            await _clinicService.AddPatientAsync(createPatientVm);

            return Ok();
        }

        [HttpPost("operation")]
        public async Task<IActionResult> CreateNewPatient(CreateOperationVm createOperationVm) {
            await _clinicService.AddOperationAsync(createOperationVm);

            return Ok();
        }
    }
}
