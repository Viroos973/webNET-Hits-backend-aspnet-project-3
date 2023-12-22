using Microsoft.AspNetCore.Mvc;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Controllers
{
    [ApiController]
    [Route("api/dictionary")]
    public class DictionaryController : ControllerBase
    {
        public readonly IDictionaryServices _dictionaryServices;

        public DictionaryController(IDictionaryServices dictionaryServices)
        {
            _dictionaryServices = dictionaryServices;
        }

        [HttpGet]
        [Route("speciality")]
        public async Task<IActionResult> GetSpecialties(string? name, int? page, int? size)
        {
            try
            {
                SpecialtiesPagedListModel specialties = await _dictionaryServices.GetSpecialytis(name, page, size);
                return Ok(specialties);
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

        [HttpGet]
        [Route("isd10")]
        public async Task<IActionResult> GetISD10(string? request, int? page, int? size)
        {
            try
            {
                Isd10SearchModel specialties = await _dictionaryServices.GetISD10(request, page, size);
                return Ok(specialties);
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
