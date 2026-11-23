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
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _InvoiceService;

        public InvoiceController(IInvoiceService InvoiceService)
        {
            _InvoiceService = InvoiceService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Create( CreateInvoiceDto dto)
        {

            try
            {
                await _InvoiceService.CreateAsync(dto);
                return StatusCode(StatusCodes.Status201Created, new { message = "Invoice created successfully." });
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

        [HttpGet]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> GetAll(int pageNumber = 1,int pageSize = 10) //////
        {

            var result = await _InvoiceService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("my-invoices")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> GetAllMyInvoices(int pageNumber = 1, int pageSize = 10) //////
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _InvoiceService.GetAllMyInvoicesAsync(userId, pageSize, pageNumber);
            return Ok(result);
        }


        [HttpGet("{invoiceId:int}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int invoiceId)
        {
            try
            {
                var invoice = await _InvoiceService.GetByIdAsync(invoiceId);
                return Ok(invoice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("order/{orderId:int}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<InvoiceResponseDto>> GetByOrderId(int orderId)
        {
            try
            {
                var invoice = await _InvoiceService.GetByOrderIdAsync(orderId);
                return Ok(invoice);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("patient/{patientId:int}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> GetByPatient(
            int patientId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _InvoiceService.GetByPatientAsync(patientId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> Search(InvoiceSearchDto filter) ////
        {
            var result = await _InvoiceService.SearchAsync(filter);
            return Ok(result);
        }



        [HttpGet("search-my")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<PagedResult<InvoiceResponseDto>>> SearchMyInvoices(InvoiceSearchDto filter) ////
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _InvoiceService.SearchMyInvoicesAsync(filter,userId);
            return Ok(result);
        }

        [HttpPatch("{invoiceId:int}/status")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<InvoiceResponseDto>> UpdateStatus(
            int invoiceId,
            InvoiceStatus status)
        {
            try
            {
                var updated = await _InvoiceService.UpdateStatusAsync(invoiceId, status);
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

        [HttpPatch("{invoiceId:int}/cancel")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<InvoiceResponseDto>> Cancel(int invoiceId)
        {
            try
            {
                var cancelled = await _InvoiceService.CancelAsync(invoiceId);
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

        [HttpGet("count-By-Status")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<ActionResult<int>> CountByStatus([FromQuery] InvoiceStatus ?status=null)
        {

                int cnt = await _InvoiceService.CountAllInvoicesByStatus(status);
                return Ok(cnt);
        }


    }
}
