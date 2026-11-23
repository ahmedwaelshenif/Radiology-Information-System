using Application.DTOs.RequestDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Create(PrescriptionRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _prescriptionService.CreateAsync(dto, userId); //update later after using authentication
            if (res)
                return Ok();

            return NotFound("Treatment Plan Not found");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> GetById(int id)
        {
            var res=await _prescriptionService.GetByIdAsync(id);
            if (res == null)
                return NotFound("Prescription not found.");
            return Ok(res);
        }

      
        [HttpPatch("cancel/{id:int}")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> CancelPrescription(int id)
        {
            try
            {
                await _prescriptionService.CancelPrescriptionAsync(id);
                return Ok();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Search(PrescriptionSearchDto searchDto)//////
        {
            var res = await _prescriptionService.SearchAsync(searchDto);
            return Ok(res);
        }

        [HttpGet("search-my-prescriptions")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SearchMyPrescriptions(PrescriptionSearchDto searchDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _prescriptionService.SearchMyPrescriptionsAsync(searchDto,userId);
            return Ok(res);
        }

        [HttpGet("get-my-prescriptions")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAllMyPrescriptions(int pageNumber,int pageSize)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _prescriptionService.GetAllMyPrescriptionsAsync(pageNumber,pageSize, userId);
            return Ok(res);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult> Update(int id, [FromBody] PrescriptionRequestDto prescriptionDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            PrescriptionUpdateResult res = await _prescriptionService.UpdateAsync(id,prescriptionDto,userId); //update later after using authentication
            if(res== PrescriptionUpdateResult.Success)
                return Ok(res);
            if (res == PrescriptionUpdateResult.PrescriptionNotFound)
                return NotFound("Prescription Not Found ");

            return NotFound("Treatment Plan Not Found ");
        }



    }
}
