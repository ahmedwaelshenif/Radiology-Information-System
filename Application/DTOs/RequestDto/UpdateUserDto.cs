using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class UpdateUserDto
    {
        public String ? Name { get; set; }
        public String? PhoneNumber { get; set; }
        public String? OldPassword { get; set; }
        public String? NewPassword { get; set; }
    }
}
