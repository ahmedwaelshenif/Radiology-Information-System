
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RadiologyInformationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TreatmentPlanController : ControllerBase
    {
        private readonly ITreatmentPlanService _treatmentPlanService;

        public TreatmentPlanController(ITreatmentPlanService treatmentPlanService)
        {
            _treatmentPlanService = treatmentPlanService;
        }

        
        [HttpPost]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Create(
            [FromBody] TreatmentPlanRequestDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _treatmentPlanService.CreateAsync(dto, userId);

            return Ok(new { message = "Treatment plan created successfully" });
        }

      
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Radiologist")]
        public async Task<ActionResult<TreatmentPlanResponseDto>> GetById(int id)
        {
            try
            {
                var result = await _treatmentPlanService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

      
        [HttpGet]
        [Authorize(Roles = "Radiologist")]
        public async Task<ActionResult<PagedResponse<TreatmentPlanResponseDto>>> Search(
            [FromQuery] TreatmentPlanSearchDto searchDto)
        {
            var result = await _treatmentPlanService.SearchAsync(searchDto);

            return Ok(result);
        }

        
        [HttpGet("patient/{patientId:int}")]
        [Authorize(Roles = "Radiologist")]
        public async Task<ActionResult<PagedResponse<TreatmentPlanResponseDto>>> GetByPatientId(
            int patientId)
        {
            var result = await _treatmentPlanService.GetByPatientIdAsync(patientId);

            return Ok(result);
        }

        
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResponse<TreatmentPlanResponseDto>>> GetMyTreatmentPlans(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _treatmentPlanService.GetAllMyTreatmentPlansAsync(
                pageNumber, pageSize, userId);

            return Ok(result);
        }

      
        [HttpGet("my/search")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResponse<TreatmentPlanResponseDto>>> SearchMyTreatmentPlans(
            [FromQuery] TreatmentPlanSearchDto searchDto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _treatmentPlanService.SearchMyTreatmentPlansAsync(
                searchDto, userId);

            return Ok(result);
        }

       
        [HttpGet("my/{id:int}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<TreatmentPlanResponseDto>> GetMyTreatmentPlanById(
            int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var result = await _treatmentPlanService.GetMyTreatmentPlanByIdAsync(id, userId);
                return Ok(result);
            }
            catch (Exception ex) {
                return NotFound(new { Message = ex.Message });
            }

         
        }

     
        [HttpPatch("{id:int}/cancel")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Cancel(int id)
        {
            try {
                await _treatmentPlanService.UpdateTreatmentPlanStatusAsync(id, false);

                return Ok(new { message = "Treatment plan cancelled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }


        [HttpPatch("{id:int}/complete")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                await _treatmentPlanService.UpdateTreatmentPlanStatusAsync(id, true);

                return Ok(new { message = "Treatment plan completed successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
    }
}
