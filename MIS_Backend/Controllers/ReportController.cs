using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/report/icdrootsreport")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(ITokenService tokenService, IReportService reportService, ILogger<ReportController> logger)
        {
            _tokenService = tokenService;
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReport([Required] DateTime start, [Required] DateTime end, [FromQuery] List<Guid> icdRoots) 
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get report with parameters: {start}, {end}, {icdRoots}");
                IcdRootsReportModel report = await _reportService.GetReport(start, end, icdRoots);

                _logger.LogInformation("Attempt to get report was successful");
                return Ok(report);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get report");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get report: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get report: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
