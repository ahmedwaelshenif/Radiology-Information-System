using Application.Common;
using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Application.Enums;
using Application.Interfaces.IServices;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Create(
             CreateRadiologyOrderDto dto,
             int orderedByUserId)
        {

            try
            {
                await _RadiologyOrderService.CreateAsync(dto, orderedByUserId);
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

        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> GetAll(int pageNumber = 1,int pageSize = 10)
        {


            var result = await _RadiologyOrderService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{orderId:int}")]
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
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> GetByPatient(
            int patientId,
            int pageNumber = 1,
            int pageSize = 10)
        {

            var result = await _RadiologyOrderService.GetByPatientAsync(patientId, pageNumber, pageSize);
            return Ok(result);
        }
        [HttpPost("search")]
        public async Task<ActionResult<PagedResult<RadiologyOrderResponseDto>>> Search([FromQuery] RadiologyOrderSearchDto filter)
        {
            var result = await _RadiologyOrderService.SearchAsync(filter);
            return Ok(result);
        }

        [HttpPatch("{orderId:int}/status")]
        public async Task<ActionResult<RadiologyOrderResponseDto>> UpdateStatus(int orderId,OrderStatus status)
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

        // POST api/radiologyorders/{orderId:int}/cancel
        [HttpPost("{orderId:int}/cancel")]
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
    }
}
