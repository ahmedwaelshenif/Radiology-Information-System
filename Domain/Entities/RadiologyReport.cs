using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RadiologyReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudyId { get; set; }
        public RadiologyStudy Study { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        [Required]
        public byte[] PdfFile { get; set; }
    }
}