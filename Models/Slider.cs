using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PustokProject.Attributes.ValidationAttributes;

namespace PustokProject.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public string Desc { get; set; }
        [MaxLength(100)]
        public string Image { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public string ButtonText { get; set; }
        [MaxLength(250)]
        public string BtnUrl { get; set; }
        [MaxFileSize(2097152)]
        [AllowedFileTypes("image/jpeg", "image/png")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }

}
