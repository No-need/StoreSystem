using Microsoft.AspNetCore.Mvc.Rendering;

namespace StoreSystem.ViewModels
{

    public class ProductViewModel
    {
        public long ProductId {  get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }
    
        public string? ImageUrl { get; set; }

        public long? ImageId { get; set; }
    }

    public class ProductDetailPageViewModel
    {
        public List<SelectListItem> CategorySelect { get; set; } = [];

        public List<SelectListItem> SizeSelect { get; set; } = [];
        public ProductDetailViewModel? Detail { get; set; }
    }

    public class ProductDetailViewModel
    {
        public long ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SKU { get; set; }

        public string Size { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Category { get; set; }
    }

    public class OrderViewModel
    {
        public long OrderId { get; set;}

        public string? OrderMember { get; set; }

        public string? OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public List<OrderProductViewModel> Products { get; set; } = [];
    }

    public class OrderProductViewModel
    {
        public long? ProductId { get; set; }

        public string? Name { get; set; }
    
        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }

    public class OrderDetailPageViewModel
    {
        public long OrderId { get; set;}

        public string? OrderMember { get; set; }

        public string? OrderDate { get; set; }

        public string? OrderStatus { get; set; }

        public string? PayMethod { get; set; }

        public string? PayStatus { get; set; }

        public string? ReceiveAddress { get; set; }

        public string? ReceiveMember { get; set; }

        public string? ReceivePhone { get; set; }

        public string? ShippingMethod { get; set; }

        public string? ShippingStatus { get;set; }

        public List<OrderProductDetailViewModel> OrderProducts { get; set; } = [];
    }

    public class OrderProductDetailViewModel
    {
        public long? ProductId { get; set; }

        public string? Name { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }

    public class ClothDataViewModel
    {
        public List<ClothImageViewModel> Images { get; set; } = [];
    }

    public class ClothImageViewModel
    {
        public string Name { get; set; }

        public string Category { get;set; }

        public string ImageUrl { get; set; }

        public Dictionary<string,long> Size { get; set; } = [];
    }
}
