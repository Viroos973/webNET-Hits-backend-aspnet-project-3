using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/dictionary")]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryServices _dictionaryServices;
        private readonly ILogger<DictionaryController> _logger;

        public DictionaryController(IDictionaryServices dictionaryServices, ILogger<DictionaryController> logger)
        {
            _dictionaryServices = dictionaryServices;
            _logger = logger;
        }

        [HttpGet]
        [Route("speciality")]
        public async Task<IActionResult> GetSpecialties(string? name, int? page = 1, int? size = 5)
        {
            try
            {
                _logger.LogInformation($"Attempt to get specialties with parameters: {name}, {page}, {size}");
                SpecialtiesPagedListModel specialties = await _dictionaryServices.GetSpecialytis(name, page, size);

                _logger.LogInformation("Attempt to get specialties was successful");
                return Ok(specialties);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get specialties: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get specialties: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("isd10")]
        public async Task<IActionResult> GetISD10(string? request, int? page = 1, int? size = 5)
        {
            try
            {
                _logger.LogInformation($"Attempt to get icd10 with parameters: {request}, {page}, {size}");
                Isd10SearchModel specialties = await _dictionaryServices.GetISD10(request, page, size);

                _logger.LogInformation("Attempt to get icd10 was successful");
                return Ok(specialties);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"Bad request exception occurred upon attempt to get icd10: {ex.Message}");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get icd10: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("isd10/roots")]
        public async Task<IActionResult> GetRootISD10()
        {
            try
            {
                _logger.LogInformation("Attempt to get roots icd10");
                List<Isd10RecordModel> specialties = await _dictionaryServices.GetRootISD10();

                _logger.LogInformation("Attempt to get roots icd10 was successful");
                return Ok(specialties);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server exception occurred upon attempt to get roots icd10: {ex.Message}");
                return StatusCode(500, new Response
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }
    }
}
