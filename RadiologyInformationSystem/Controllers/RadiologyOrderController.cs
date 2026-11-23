using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RadiologyInformationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RadiologyOrderController : ControllerBase
    {
        private readonly IRadiologyOrderService _RadiologyOrderService;

        public RadiologyOrderController(IRadiologyOrderService RadiologyOrderService)
        {
            _RadiologyOrderService = RadiologyOrderService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(
             CreateRadiologyOrderDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                await _RadiologyOrderService.CreateAsync(dto, userId);
                return StatusCode(StatusCodes.Status201Created, new { message = "Radiology order created successfully." });
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
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> GetAll(int pageNumber = 1,int pageSize = 10)
        {


            var result = await _RadiologyOrderService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{orderId:int}")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<RadiologyOrderResponseDto>> GetById(int orderId)
        {
            try
            {
                var order = await _RadiologyOrderService.GetByIdAsync(orderId);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("accession/{accessionNumber}")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<RadiologyOrderResponseDto>> GetByAccessionNumber(string accessionNumber)
        {
            try
            {
                var order = await _RadiologyOrderService.GetByAccessionNumberAsync(accessionNumber);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("patient/{patientId:int}")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> GetByPatient(
            int patientId,
            int pageNumber = 1,
            int pageSize = 10)
        {

            var result = await _RadiologyOrderService.GetByPatientAsync(patientId, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Receptionist,Radiologist")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> Search([FromQuery] RadiologyOrderSearchDto filter)
        {
            var result = await _RadiologyOrderService.SearchAsync(filter);
            return Ok(result);
        }

        [HttpPatch("{orderId:int}/status")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult<RadiologyOrderResponseDto>> UpdateStatus(int orderId, [FromQuery] OrderStatus status)
        {
            try
            {
                var updated = await _RadiologyOrderService.UpdateStatusAsync(orderId, status);
                return Ok(updated);
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


        [HttpPatch("{orderId:int}/cancel")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult<RadiologyOrderResponseDto>> Cancel(int orderId)
        {
            try
            {
                var cancelled = await _RadiologyOrderService.CancelAsync(orderId);
                return Ok(cancelled);
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

        [HttpGet("my-orders")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> GetMyOrders(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _RadiologyOrderService.GetAllMyOrdersPagedAsync(
                userId, pageNumber, pageSize);

            return Ok(result);
        }

        [HttpGet("my-orders/search")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> SearchMyOrders(
               [FromQuery] RadiologyOrderSearchDto filter)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _RadiologyOrderService.SearchMyOrdersAsync(userId, filter);

            return Ok(result);
        }
    }
}
