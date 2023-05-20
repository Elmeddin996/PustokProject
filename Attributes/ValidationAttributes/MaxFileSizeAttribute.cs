using System.ComponentModel.DataAnnotations;

namespace PustokProject.Attributes.ValidationAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;

        public MaxFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<IFormFile> files = new List<IFormFile>();


            if (value is IFormFile)
                files.Add((IFormFile)value);
            else if (value is List<IFormFile>)
                files = value as List<IFormFile>;

            foreach (var item in files)
            {
                if (item.Length > _maxSize)
                    return new ValidationResult("FileSize must be less or equal than " + _maxSize + " byte");
            }

            return ValidationResult.Success;
        }
    }
}
