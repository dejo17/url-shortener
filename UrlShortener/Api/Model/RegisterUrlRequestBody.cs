using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener
{
    public class RegisterUrlRequestBody : IValidatableObject //da bi mogli raditi custom validation
    {
        [Required]
        [StringLength(500)]
        [Url]
        public string url { get; set; }
        public int redirectType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((redirectType > 0 && !(redirectType == 301 || redirectType == 302)))
            {
                {
                    yield return new ValidationResult(
                        $"Redirect type is optional. If present must be either 301 or 302",
                        new[] { nameof(redirectType) });
                }
            }
        }
    }
}
