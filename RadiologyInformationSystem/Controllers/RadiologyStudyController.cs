using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RadiologyStudyController : ControllerBase
    {
        private readonly IRadiologyStudyService _radiologyStudyService;

        public RadiologyStudyController(IRadiologyStudyService radiologyStudyService)
        {
            _radiologyStudyService = radiologyStudyService;
        }

        [HttpPatch("status/{id:int}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StudyStatus status)
        {
            if (!Enum.IsDefined(typeof(StudyStatus), status))
                return BadRequest("Invalid study status.");
            try
            {
                await _radiologyStudyService.UpdateStatusAsync(status, id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(RadiologyStudySearchDto searchDto)
        {
            var res = await _radiologyStudyService.SearchAsync(searchDto);
            return Ok(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _radiologyStudyService.GetByIdAsync(id);
            if (res == null)
                return NotFound("Radiology study not found.");
            return Ok(res);
        }
    }
}
