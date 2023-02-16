namespace commerce_challenge.Models.ViewModels
{
    public record ProductViewModel
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Size { get; set; }
        public required int Stars { get; set; }
    }
}
