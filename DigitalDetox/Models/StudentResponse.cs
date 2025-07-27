using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalDetox.Models
{
    public class StudentResponse
    {
        [Key]
        public int ResponseId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public bool Answer { get; set; }  // true = Yes, false = No

        [Required, Column(TypeName = "date")]
        public DateTime WeekStartDate { get; set; } // e.g., always Monday

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public Question Question { get; set; } = null!;
    }
}
