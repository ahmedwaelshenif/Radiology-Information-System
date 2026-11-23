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

       

        [HttpGet]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> GetAll(int pageNumber = 1,int pageSize = 10) 
        {

            var result = await _InvoiceService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("my-invoices")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAllMyInvoices(int pageNumber = 1, int pageSize = 10) 
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _InvoiceService.GetAllMyInvoicesAsync(userId, pageSize, pageNumber);
            return Ok(result);
        }


        [HttpGet("{invoiceId:int}")]
        [Authorize(Roles = "Admin,Radiologist")]
        public async Task<IActionResult> GetById(int invoiceId)
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
        public async Task<IActionResult> GetByOrderId(int orderId)
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
        public async Task<IActionResult> GetByPatient(
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
        public async Task<IActionResult> Search(InvoiceSearchDto filter) 
        {
            var result = await _InvoiceService.SearchAsync(filter);
            return Ok(result);
        }



        [HttpGet("search-my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> SearchMyInvoices(InvoiceSearchDto filter) 
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _InvoiceService.SearchMyInvoicesAsync(filter,userId);
            return Ok(result);
        }


        [HttpGet("sum-amount")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> SumAmountWithin(DateTime start,DateTime end) 
        {
            try
            {
                decimal sum=await _InvoiceService.CountAmountWithin(start,end);
                return Ok(sum);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
