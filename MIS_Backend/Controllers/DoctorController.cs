using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorSevise;
        private readonly ITokenService _tokenService;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorService doctorSevise, ITokenService tokenService, ILogger<DoctorController> logger)
        {
            _doctorSevise = doctorSevise;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> UserRegister(DoctorRegisterModel userRegisterModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {userRegisterModel}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Attempt to register doctor with parameters: {userRegisterModel}");
                TokenResponseModel token = await _doctorSevise.RegisterUser(userRegisterModel);

                _logger.LogInformation("Attempt to register doctor was successful");
                return Ok(token);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to reqistrer doctor: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to register doctor: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginCredentialsModel credentials)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {credentials}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Attempt to login with parameters: {credentials}");
                TokenResponseModel token = await _doctorSevise.Login(credentials);

                _logger.LogInformation("Attempt to login was successful");
                return Ok(token);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to login: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to login: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                string token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length);
                await _tokenService.CheckToken(token);

                _logger.LogInformation($"Attempt to logout with parameters: {token}");
                await _doctorSevise.LogOut(token);

                _logger.LogInformation("Attempt to logout was successful");
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to logout");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to logout: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to logout");
                DoctorModel user = await _doctorSevise.GetProfile(Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to get profile was successful");
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to get profile");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to get profile: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get profile: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Authorize]
        [Route("profile")]
        public async Task<IActionResult> EditProfile(DoctorEditModel editModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Bad request exception occurred in the body {editModel}");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Attempt to check token for user authorization");
                await _tokenService.CheckToken(HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));

                _logger.LogInformation($"Attempt to edit profile with parameters: {editModel}");
                await _doctorSevise.EditProfile(editModel, Guid.Parse(User.Identity.Name));

                _logger.LogInformation("Attempt to edit profile was successful");
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("User is not authorized to edit profile");
                return Unauthorized(new Response
                {
                    Status = "Error",
                    Message = "User is not authorized"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to edit profile: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Key not found exception occurred upon attempt to edit profile: {ex.Message}");
                return NotFound(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to edit profile: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
