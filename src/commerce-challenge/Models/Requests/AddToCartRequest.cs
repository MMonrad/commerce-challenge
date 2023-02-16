using System.ComponentModel.DataAnnotations;

namespace commerce_challenge.Models.Requests
{
    public record AddToCartRequest
    {
        [Required]
        [MinLength(1)]
        public required string ProductId { get; init; }

        [Required]
        public required uint Quantity { get; init; }
    }
}
