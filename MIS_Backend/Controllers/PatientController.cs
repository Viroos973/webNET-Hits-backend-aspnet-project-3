using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.Database.Enums;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientSevise;
        private readonly ITokenService _tokenService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService patientSevise, ITokenService tokenService, ILogger<PatientController> logger)
        {
            _patientSevise = patientSevise;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePatient(PatientCreateModel patient)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {patient}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to create patient with parameters: {patient}");
                Guid patientId = await _patientSevise.CreatePatient(patient);

                _logger.LogInformation("Attempt to create patient was successful");
                return Ok(patientId);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to create patient");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to create patient: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to create patient: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPatient(string? name, [FromQuery] List<Conclusion> conclusions, PatientSorting? sorting, bool? scheduledVisits = false,
            bool? onlyMine = false, int? page = 1, int? size = 5)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get patients with parameters: {name}, {conclusions}, {sorting}, {scheduledVisits}, {onlyMine}, {page}, {size}");
                PatientPagedListModel patients = await _patientSevise.GetPatient(Guid.Parse(User.Identity.Name), name, conclusions, sorting, scheduledVisits, onlyMine, page, size);

                _logger.LogInformation("Attempt to get patients was successful");
                return Ok(patients);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get patients");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get patients: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get patients: {ex.Message}");
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
                _logger.LogError($"Bad request exception occurred in the body {inspection}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to create inspection with parameters: {inspection}, {id}");
                Guid inspectionId = await _patientSevise.CreateInspection(inspection, id, Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to create inspection was successful");
                return Ok(inspectionId);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to create inspection");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to create inspection: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to create inspection: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{id}/inspection")]
        public async Task<IActionResult> GetInspection(Guid id, [FromQuery] List<Guid> icdRoots, bool? grouped = false, int? page = 1, int? size = 5)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get inspections with parameters: {id}, {grouped}, {icdRoots}, {page}, {size}");
                InspectionPagedListModel inspections = await _patientSevise.GetInspections(id, grouped, icdRoots, page, size);

                _logger.LogInformation("Attempt to get inspections was successful");
                return Ok(inspections);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get inspections");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get inspections: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get inspections: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get inspections: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetSpecificPatient(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get specific patient with parameters: {id}");
                PatientModel patient = await _patientSevise.GetSpecificPatient(id);

                _logger.LogInformation("Attempt to get specific patient was successful");
                return Ok(patient);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get specific patient");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get specific patient: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get specific patient: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{id}/inspection/search")]
        public async Task<IActionResult> GetInspectionWithoutChild(Guid id, string? request)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get inspection without child with parameters: {id}, {request}");
                List<InspectionShortModel> inspections = await _patientSevise.GetInspectionWithoutChild(id, request);

                _logger.LogInformation("Attempt to get inspection without child was successful");
                return Ok(inspections);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get inspection without child");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get inspection without child: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get inspection without child: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
