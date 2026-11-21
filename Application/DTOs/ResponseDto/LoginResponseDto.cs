using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDto
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
