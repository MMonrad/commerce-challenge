using System.ComponentModel.DataAnnotations;

namespace commerce_challenge.Models.Requests
{
    public record CreateCartRequest
    {
        [Required]
        [MinLength(1)]
        public string Email { get; set; }
    }
}
