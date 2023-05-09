using System.ComponentModel.DataAnnotations;

namespace PustokProject.Models
{
    public class Setting
    {
        [Required] 
        [MaxLength(25)]
        public string Key { get; set; }
        [MaxLength(250)]

        public string Value { get; set; }
    }
}
