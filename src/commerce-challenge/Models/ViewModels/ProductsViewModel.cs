using System.Collections.Generic;

namespace commerce_challenge.Models.ViewModels
{
    public record ProductsViewModel
    {
        public required IReadOnlyCollection<ProductViewModel> Products { get; init; } 
    }
}
