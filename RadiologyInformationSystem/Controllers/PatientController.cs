using Application.DTOs.RequestDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<IActionResult> GetAll(int pageNumber=1, int pageSize=10) {
            var result = await _patientService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult>Register(RegisterPatientDto dto)
        {
            RegisterResult res= await _patientService.Register(dto);
            if (res == RegisterResult.Success)
                return Ok("Patient registered successfully.");

            if (res == RegisterResult.EmailAlreadyExists)
                return Conflict("Email already exists.");

            return BadRequest("Registration failed.");

        }

        [HttpPost ("search")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<IActionResult> Search(PatientSearchDto searchDto)
        {
            var res=await _patientService.SearchAsync(searchDto);
            return Ok(res);
        }

    }
}
