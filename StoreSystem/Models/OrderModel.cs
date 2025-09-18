namespace StoreSystem.Models
{
    public class OrderCreateModel
    {
        public string? OrderMember { get; set; }

        public string? ReceiveMember {  get; set; }

        public string? ReceiveAddress { get; set; }

        public string? ReceivePhone { get; set; }

        public string? PayMethod { get; set; }

        public string? ShippingMethod { get; set; }

        public List<OrderProductModel> OrderProducts { get; set; } = [];
    }

    public class OrderProductModel
    {
        public long ProductId { get; set; }
    
        public int Quantity { get; set; }
    }

    public class OrderUpdateModel
    {

    }
}
