using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDto
{
    public class ReceptionistSearchDto : PagedRequestDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public bool?IsActive { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }


    }
}
