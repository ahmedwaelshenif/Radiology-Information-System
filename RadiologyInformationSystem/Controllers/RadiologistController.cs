using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RadiologyInformationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
  
    public class RadiologistController : ControllerBase
    {
        private readonly IRadiologistService _RadiologistService;

        public RadiologistController(IRadiologistService RadiologistService)
        {
            _RadiologistService = RadiologistService;
        }


        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterRadiologistDto radiologistDto)
        { 
            var result = await _RadiologistService.Register(radiologistDto);

            if (result == RegisterResult.Success)
                return StatusCode(StatusCodes.Status201Created, new { message = "Radiologist registered successfully." });

            else if (result == RegisterResult.EmailAlreadyExists)
                return Conflict(new { message = "A user with this email already exists." });

            else if (result == RegisterResult.LicenseAlreadyExists)
                return Conflict(new { message = "A radiologist with this license number already exists." });

            return BadRequest(new { message = "Unable to register radiologist." });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<PagedResponse<RadiologistResponseDto>>> GetAll([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        { 
            var response = await _RadiologistService.GetAllAsync(pageNumber, pageSize);
            return Ok(response);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<PagedResponse<RadiologistResponseDto>>> Search(RadiologistSearchDto searchDto)
        {
            var response = await _RadiologistService.SearchAsync(searchDto);
            return Ok(response);
        }
    }
}
