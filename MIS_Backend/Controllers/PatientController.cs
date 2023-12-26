using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> CreatePatient(PatientCreateModel patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));
                Guid patientId = await _patientSevise.CreatePatient(patient);
                return Ok(patientId);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
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

        [HttpPost]
        [Authorize]
        [Route("{id}/inspection")]
        public async Task<IActionResult> CreateInspection(InspectionCreateModel inspection, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));
                Guid inspectionId = await _patientSevise.CreateInspection(inspection, id, Guid.Parse(User.Identity.Name));
                return Ok(inspectionId);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
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
