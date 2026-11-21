using Application.DTOs.RequestDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrescriptionRequestDto dto)
        {
            var res = await _prescriptionService.CreateAsync(dto, 1); //update later after using authentication
            if (res)
                return Ok();

            return Conflict("No Treatment Plan Exist");
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res=await _prescriptionService.GetByIdAsync(id);
            if (res == null)
                return NotFound("Prescription not found.");
            return Ok(res);
        }

        [HttpGet("patient/{id:int}")]
        public async Task<IActionResult> GetByPatientId(int id)
        {
            var res = await _prescriptionService.GetByPatientIdAsync(id);
            if (res == null)
                return NotFound("Prescription not found.");
            return Ok(res);
        }

        [HttpGet("treatmentPlan/{id:int}")]
        public async Task<IActionResult> GetByTreatmentPlanId(int id)
        {
            var res = await _prescriptionService.GetByTreatmentPlanIdAsync(id);
            if (res == null)
                return NotFound("Prescription not found.");
            return Ok(res);
        }

        [HttpPatch("cancel/{id:int}")]
        public async Task<IActionResult> CancelPrescription(int id)
        {
            var res = await _prescriptionService.CancelPrescriptionAsync(id);
            if(res)
                return Ok(res);
            return NotFound("Prescription not found.");

        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(PrescriptionSearchDto searchDto)
        {
            var res = await _prescriptionService.SearchAsync(searchDto);
            return Ok(res);
        }

        [HttpPatch("{id:int}")]

        public async Task<IActionResult> Update(int id, [FromBody] PrescriptionRequestDto prescriptionDto)
        {
            PrescriptionUpdateResult res = await _prescriptionService.UpdateAsync(id,prescriptionDto,1); //update later after using authentication
            if(res== PrescriptionUpdateResult.Success)
                return Ok(res);
            if (res == PrescriptionUpdateResult.PrescriptionNotFound)
                return NotFound("Prescription Not Found ");

            return NotFound("Treatment Plan Not Found ");
        }



    }
}
