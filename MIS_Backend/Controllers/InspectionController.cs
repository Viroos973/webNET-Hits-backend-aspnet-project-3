using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.Security;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/inspection")]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionSevise;
        private readonly ITokenService _tokenService;
        private readonly ILogger<InspectionController> _logger;

        public InspectionController(IInspectionService inspectionSevise, ITokenService tokenService, ILogger<InspectionController> logger)
        {
            _inspectionSevise = inspectionSevise;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetSpecificInspection(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get specific inspection with parameters: {id}");
                InspectionModel inspection = await _inspectionSevise.GetSpecificInspection(id);

                _logger.LogInformation("Attempt to get specific inspection was successful");
                return Ok(inspection);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get specific inspection");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get specific inspection: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get specific inspection: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> EditSpecificInspection(InspectionEditModel inspectionEdit, Guid id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {inspectionEdit}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to edit specific inspection with parameters: {id}, {inspectionEdit}");
                await _inspectionSevise.EdiInspection(inspectionEdit, id, Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to edit specific inspection was successful");
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to edit specific inspection");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to edit specific inspection: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (SecurityException ex)
            {
                _logger.LogError($"Security exception occurred upon attempt to edit specific inspection: {ex.Message}");
                return StatusCode(403, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to edit specific inspection: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to edit specific inspection: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{id}/chain")]
        public async Task<IActionResult> GetInspectionChain(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt toget inspection chain with parameters: {id}");
                List<InspectionPreviewModel> inspection = await _inspectionSevise.GetInspectionChain(id);

                _logger.LogInformation("Attempt to get inspection chain was successful");
                return Ok(inspection);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get inspection chain");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get inspection chain: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get inspection chain: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get inspection chain: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
