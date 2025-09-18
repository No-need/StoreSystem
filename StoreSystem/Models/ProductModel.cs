namespace StoreSystem.Models
{
    public class ProductCreateModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string SKU { get;set; }

        public string Size { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Category {  get; set; }

        public List<IFormFile> Images { get; set; } = [];
    }

    public class ProductUpdateModel: ProductCreateModel
    {
        public long ProductId {  get; set; }

        public List<long> DeleteFiles { get; set; } = [];
    }

    public class ClothSearchCondModel
    {
        public int? Index { get; set; }
    }
}
