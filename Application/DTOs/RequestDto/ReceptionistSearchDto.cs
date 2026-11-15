using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class ReceptionistSearchDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
