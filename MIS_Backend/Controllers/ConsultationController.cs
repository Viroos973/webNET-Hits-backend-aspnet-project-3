using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.Security;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/consultation")]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationSevise;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(IConsultationService consultationSevise, ITokenService tokenService, ILogger<ConsultationController> logger)
        {
            _consultationSevise = consultationSevise;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetInspectionForCosultation([FromQuery] List<Guid> icdRoots, bool? grouped = false, int? page = 1, int? size = 5)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get inspection for consultation with parameters: {grouped}, {icdRoots}, {page}, {size}");
                InspectionPagedListModel inspections = await _consultationSevise.GetInspectionForConsultation(Guid.Parse(User.Identity.Name), grouped, icdRoots, page, size);

                _logger.LogInformation("Attempt to get inspection for consultation was successful");
                return Ok(inspections);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get inspection for consultation");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get inspection for consultation: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get inspection for consultation: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get inspection for consultation: {ex.Message}");
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
        public async Task<IActionResult> GetConsultation(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to get consultation with parameters: {id}");
                ConsultationModel consultation = await _consultationSevise.GetConsultation(id);

                _logger.LogInformation("Attempt to get consultation was successful");
                return Ok(consultation);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get consultation");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get consultation: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get consultation: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("{id}/comment")]
        public async Task<IActionResult> AddComment(Guid id, CommentCreateModel comment)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {comment}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to add comment with parameters: {id}, {comment}");
                Guid commentId = await _consultationSevise.AddComment(id, comment, Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to add comment was successful");
                return Ok(commentId);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to add comment");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to add comment: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to add comment: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (SecurityException ex)
            {
                _logger.LogError($"Security exception occurred upon attempt to add comment: {ex.Message}");
                return StatusCode(403, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to add comment: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Authorize]
        [Route("comment/{id}")]
        public async Task<IActionResult> EditComment(Guid id, InspectionCommentCreateModel comment)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {comment}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to edit comment with parameters: {id}, {comment}");
                await _consultationSevise.EditComment(id, comment, Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to edit was successful");
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to edit comment");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to edit comment: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (SecurityException ex)
            {
                _logger.LogError($"Security exception occurred upon attempt to get to edit comment: {ex.Message}");
                return StatusCode(403, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to edit comment: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
