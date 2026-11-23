using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] StudyStatus status)
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

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<IActionResult> Search([FromQuery]  RadiologyStudySearchDto searchDto)
        {
            var res = await _radiologyStudyService.SearchAsync(searchDto);
            return Ok(res);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _radiologyStudyService.GetByIdAsync(id);
            if (res == null)
                return NotFound("Radiology study not found.");
            return Ok(res);
        }


        [HttpGet("my-studies")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyStudies(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _radiologyStudyService.GetMyStudiesAsync(
                pageSize, pageNumber, userId);

            return Ok(result);
        }

        [HttpGet("my-studies/search")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SearchMyStudies(
            [FromQuery] RadiologyStudySearchDto searchDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _radiologyStudyService.SearchMyStudiesAsync(
                userId, searchDto);

            return Ok(result);
        }


        [HttpGet("my-studies/{id:int}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyStudyById(int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var result = await _radiologyStudyService
                    .GetMyStudyByIdAsync(id, userId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Count-By-Status")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<IActionResult> CountAllByStatus([FromQuery] StudyStatus ?status=null)
        {
            var res = await _radiologyStudyService.CountByStatus(status);
            return Ok(res);
        }
    }
}
