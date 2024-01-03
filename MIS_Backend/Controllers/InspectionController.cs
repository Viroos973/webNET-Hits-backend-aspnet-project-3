using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/inspection")]
    public class InspectionController : ControllerBase
    {
        public readonly IInspectionService _inspectionSevise;
        public readonly ITokenService _tokenService;

        public InspectionController(IInspectionService inspectionSevise, ITokenService tokenService)
        {
            _inspectionSevise = inspectionSevise;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetSpecificInspection(Guid id)
        {
            try
            {
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));
                InspectionModel inspection = await _inspectionSevise.GetSpecificInspection(id);
                return Ok(inspection);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new Response
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
