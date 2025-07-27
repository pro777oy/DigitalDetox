using System.ComponentModel.DataAnnotations;

namespace DigitalDetox.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Class { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Roll { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<StudentResponse> StudentResponses { get; set; } = new List<StudentResponse>();
    }
}
