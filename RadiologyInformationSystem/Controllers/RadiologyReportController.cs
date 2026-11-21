using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RadiologyInformationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RadiologyReportController : ControllerBase
    {
        private readonly IRadiologyReportService _RadiologyReportService;

        public RadiologyReportController(IRadiologyReportService RadiologyReportService)
        {
            _RadiologyReportService = RadiologyReportService;
        }

        [HttpPost("Create")]

        public async Task<IActionResult> Create(CreateRadiologyReportDto dto,int createdByUserId)
        {

            try
            {
                await _RadiologyReportService.CreateAsync(dto, createdByUserId);
                return StatusCode(StatusCodes.Status201Created, new { message = "Radiology report created successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]

        public async Task<ActionResult<PagedResult<RadiologyReportResponseDto>>> GetAll(int pageNumber = 1,int pageSize = 10)
        {

            var result = await _RadiologyReportService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{reportId:int}")]
        public async Task<ActionResult<RadiologyReportResponseDto>> GetById(int reportId)
        {
            try
            {
                var report = await _RadiologyReportService.GetByIdAsync(reportId);
                return Ok(report);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("{reportId:int}/pdf")]
        public async Task<IActionResult> GetPdf(int reportId)
        {
            try
            {
                var pdfBytes = await _RadiologyReportService.GetPdfAsync(reportId);
                return File(pdfBytes, "application/pdf", $"radiology-report-{reportId}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<RadiologyReportResponseDto>>> Search(RadiologyReportSearchDto filter)
        {
            var result = await _RadiologyReportService.SearchAsync(filter);
            return Ok(result);
        }

        [HttpPut("{reportId:int}")]
        public async Task<IActionResult> Update(int reportId,UpdateRadiologyReportDto dto)
        {
            try
            {
                await _RadiologyReportService.UpdateAsync(reportId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{reportId:int}")]

        public async Task<IActionResult> Delete(int reportId)
        {
            try
            {
                await _RadiologyReportService.DeleteAsync(reportId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
