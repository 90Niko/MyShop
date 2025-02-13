using System.Data;

namespace MyShop.DTO.ModelsDto
{
    public class OrderModel
    {
        public int UserId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public string Address { get; set; } = string.Empty;

        public List<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();

    }
}
