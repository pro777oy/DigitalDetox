using System.ComponentModel.DataAnnotations;

namespace DigitalDetox.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required, MaxLength(500)]
        public string QuestionText { get; set; } = null!;

        public ICollection<StudentResponse> StudentResponses { get; set; } = new List<StudentResponse>();
    }
}
