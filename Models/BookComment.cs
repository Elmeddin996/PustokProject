
using System.ComponentModel.DataAnnotations;

namespace PustokProject.Models
{
    public class BookComment
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int BookId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Text { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser AppUser { get; set; }
        public Book Book { get; set; }
    }
}