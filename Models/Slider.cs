using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PustokProject.Models
{
    public class Slider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Image { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public string ButtonText { get; set; } 
       
    }

}
