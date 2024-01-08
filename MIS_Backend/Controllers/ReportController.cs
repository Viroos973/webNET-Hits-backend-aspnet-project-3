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
        public readonly IReportService _reportService;
        public readonly ITokenService _tokenService;

        public ReportController(ITokenService tokenService, IReportService reportService)
        {
            _tokenService = tokenService;
            _reportService = reportService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReport([Required] DateTime start, [Required] DateTime end, [FromQuery] List<Guid> icdRoots) 
        {
            try
            {
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));
                IcdRootsReportModel report = await _reportService.GetReport(start, end, icdRoots);
                return Ok(report);
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
