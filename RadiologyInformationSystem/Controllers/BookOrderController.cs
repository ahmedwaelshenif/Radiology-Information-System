using Application.DTOs.RequestDto;
using Application.Interfaces.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookOrderController : ControllerBase
    {
        private readonly IBookOrderService _bookOrderService;

        public BookOrderController(IBookOrderService bookOrderService)
        {
            _bookOrderService = bookOrderService;
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto pagination)
        {
            try
            {
                var result = await _bookOrderService.GetAllAsync(pagination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _bookOrderService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] BookOrderSearchDto dto)
        {
            try
            {
                var result = await _bookOrderService.SearchAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{id:int}/accept")]
        public async Task<IActionResult> Accept(
            int id)
        {
            try
            {
                int userId=GetUserId();
                await _bookOrderService.AcceptAsync(id,userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(
            int id)
        {
            try
            {
                await _bookOrderService.RejectAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpGet("count/status/{status}")]
        public async Task<IActionResult> CountByStatus(BookOrderStatus status)
        {
            try
            {
                var count = await _bookOrderService.CountByStatusAsync(status);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookOrderDto dto)
        {
            try
            {
                int userId = GetUserId();

                await _bookOrderService.CreateAsync(dto, userId);

                return Ok(new { message = "Book order created successfully." });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookOrders(
            [FromQuery] PagedRequestDto pagination)
        {
            try
            {
                int userId = GetUserId();

                var result = await _bookOrderService
                    .GetAllMyBookOrdersAsync(userId, pagination);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my/search")]
        public async Task<IActionResult> SearchMyBookOrders(
            [FromQuery] BookOrderSearchDto dto)
        {
            try
            {
                int userId = GetUserId();

                var result = await _bookOrderService
                    .SearchMyBookOrdersAsync(dto, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my/{id:int}")]
        public async Task<IActionResult> GetMyBookOrderById(int id)
        {
            try
            {
                int userId = GetUserId();

                var result = await _bookOrderService.GetMyById(id, userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("my/{id:int}/cancel")]
        public async Task<IActionResult> CancelMyBookOrder(int id)
        {
            try
            {
                int userId = GetUserId();

                await _bookOrderService.CancelMyBookOrderAsync(id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my/count/status/{status}")]
        public async Task<IActionResult> CountMyBookOrdersByStatus(
            BookOrderStatus status)
        {
            try
            {
                int userId = GetUserId();

                var count = await _bookOrderService
                    .CountMyBookOrdersByStatusAsync(status, userId);

                return Ok(count);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        //Helper Methods
        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) ||
                !int.TryParse(userId, out int id))
            {
                throw new UnauthorizedAccessException("Invalid user identity.");
            }

            return id;
        }

        private IActionResult HandleException(Exception ex)
        {
            return ex switch
            {
                KeyNotFoundException =>
                    NotFound(new { message = ex.Message }),

                InvalidOperationException =>
                    BadRequest(new { message = ex.Message }),

                ArgumentException =>
                    BadRequest(new { message = ex.Message }),

                UnauthorizedAccessException =>
                    Unauthorized(new { message = ex.Message }),

                _ => StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                })
            };
        }
    }
}