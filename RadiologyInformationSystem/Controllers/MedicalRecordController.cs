using Application.DTOs.RequestDto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpPost]
        [Authorize(Roles = "Radiologist")]
        public async Task<IActionResult>Create(MedicalRecordRequestDto medicalRecordRequestDto)
        {
            var userId = int.Parse( User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _medicalRecordService.CreateAsync(medicalRecordRequestDto, userId); ////// update later from authentication
            return Ok();
        }

        [HttpPost("search")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> Search(MedicalRecordSearchDto searchDto)
        {
            var res=await _medicalRecordService.SearchAsync(searchDto);
            return Ok(res);
        }

        [HttpGet("search-my-records")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SearchMyRecords(MedicalRecordSearchDto searchDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _medicalRecordService.SearchMyRecordsAsync(userId,searchDto);
            return Ok(res);
        }

        [HttpGet("my-records")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyRecords(int pageNumber,int pageSize)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var res = await _medicalRecordService.GetMyRecordsAsync(userId,pageNumber,pageSize);
            return Ok(res);
        }
    }
}
