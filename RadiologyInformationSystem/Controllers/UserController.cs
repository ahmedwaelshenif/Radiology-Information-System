using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace RadiologyInformationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPatch("Activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate([FromQuery] int id)
        {

            try
            {
                await _userService.ActivateAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("DeActivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeActivate([FromQuery] int id)
        {

            try
            {
                await _userService.DeactivateAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpGet("Count-Radioligists")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountRadioligists()
        {

                var result =await _userService.CountRadiologistsAsync();
                return Ok(result);
            

        }
        [HttpGet("Count-Patients")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountPatients()
        {

                var result =await _userService.CountPatientsAsync();
                return Ok(result);
            

        }

        [HttpGet("Count-Receptionists")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountReceptionists()
        {

                var result =await _userService.CountReceptionistsAsync();
                return Ok(result);
        }

        [HttpPost("Update-User")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto,int id)
        {
            try
            {
                await _userService.UpdateUserInformation(dto, id);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpPost("Update-My-User")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist,Patient")]
        public async Task<IActionResult> UpdateMyUser(UpdateUserDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                await _userService.UpdateMyInformation(dto, userId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return NoContent();
        }

       




        
        }
    }

