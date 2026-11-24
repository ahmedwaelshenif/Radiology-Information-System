using Application.DTOs.RequestDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _receptionistService;

        public ReceptionistController(IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _receptionistService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);

        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterReceptionistDto dto)
        {
            RegisterResult res = await _receptionistService.Register(dto);
            if (res == RegisterResult.Success)
                return Ok("Receptionist registered successfully.");

            if (res == RegisterResult.EmailAlreadyExists)
                return Conflict("Email already exists.");

            return BadRequest("Registration failed.");

        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Search([FromQuery] ReceptionistSearchDto searchDto)
        {
            var res = await _receptionistService.SearchAsync(searchDto);
            return Ok(res);
        }
    }
}
