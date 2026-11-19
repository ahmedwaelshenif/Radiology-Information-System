using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class StudyUID
    {
        [Key]
        public int Id { get; set; }
        public Modality Modality { get; set; }
        public string StudyInstanceUID { get; set; }
    }
}
