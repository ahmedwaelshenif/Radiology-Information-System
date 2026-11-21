using Application.DTOs.RequestDto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult>Create(MedicalRecordRequestDto medicalRecordRequestDto)
        {
            await _medicalRecordService.CreateAsync(medicalRecordRequestDto, 1); ////// update later from authentication
            return Ok();
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(MedicalRecordSearchDto searchDto)
        {
            var res=await _medicalRecordService.SearchAsync(searchDto);
            return Ok(res);
        }
    }
}
