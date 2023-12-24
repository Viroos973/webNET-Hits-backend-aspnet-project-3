using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        public readonly IPatientService _patientSevise;
        public readonly ITokenService _tokenService;

        public PatientController(IPatientService patientSevise, ITokenService tokenService)
        {
            _patientSevise = patientSevise;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient(PatientCreateModel patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Guid patientId = await _patientSevise.CreatePatient(patient);
                return Ok(patientId);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
