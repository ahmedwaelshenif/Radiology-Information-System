using Application.DTOs.RequestDto;
using Application.DTOs.ResponseDto;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IBookOrderService
    {
        Task CreateAsync(CreateBookOrderDto dto, int userId);
        Task<PagedResponse<BookOrderResponseDto>> GetAllAsync(PagedRequestDto pagination);
        Task<PagedResponse<BookOrderResponseDto>> GetAllMyBookOrdersAsync(int userId, PagedRequestDto pagination);
        Task<PagedResponse<BookOrderResponseDto>> SearchAsync(BookOrderSearchDto dto);
        Task<PagedResponse<BookOrderResponseDto>> SearchMyBookOrdersAsync(BookOrderSearchDto dto,int userId);
        Task<BookOrderResponseDto> GetById(int id);
        Task<BookOrderResponseDto> GetMyById(int id,int userId);
        Task AcceptAsync(int id,int userId);
        Task RejectAsync(int id);
        Task CancelMyBookOrderAsync(int id, int userId);
        Task<int> CountByStatusAsync(BookOrderStatus status);
        Task<int> CountMyBookOrdersByStatusAsync(BookOrderStatus status, int userId);
    }
}
