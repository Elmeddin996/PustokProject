using System.ComponentModel.DataAnnotations;

namespace PustokProject.Models
{
    public class Author
    {

        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string FullName { get; set; }

        public List<Book> Books { get; set; }
    }
}
