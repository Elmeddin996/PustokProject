using PustokProject.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokProject.Models
{
    public class Author
    {

        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string FullName { get; set; } 
        public string ImageName { get; set; }

        [MaxFileSize(2097152)]
        [AllowedFileTypes("image/jpeg", "image/png")]
        [NotMapped]
        public IFormFile AutorImageFile { get; set; }
      


        public List<Book> Books { get; set; }=new List<Book>();
    }
}
