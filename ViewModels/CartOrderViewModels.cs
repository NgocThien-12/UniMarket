using System.ComponentModel.DataAnnotations;
using UniMarket.Models;

namespace UniMarket.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public int MaxQuantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public string SellerName { get; set; } = string.Empty;
    }

    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.Subtotal);
        public int TotalQuantity => Items.Sum(i => i.Quantity);
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
        [StringLength(100, ErrorMessage = "Họ tên không vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên người nhận")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại liên hệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng/điểm hẹn.")]
        [StringLength(300, ErrorMessage = "Địa chỉ không vượt quá 300 ký tự.")]
        [Display(Name = "Địa chỉ nhận hàng (Ký túc xá, phòng trọ, cổng trường...)")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự.")]
        [Display(Name = "Ghi chú đơn hàng (Thời gian hẹn, lưu ý...)")]
        public string? Notes { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD"; // Tiền mặt khi nhận hàng (COD)

        public CartViewModel Cart { get; set; } = new();
    }

    public class OrderDetailViewModel
    {
        public Order Order { get; set; } = null!;
        public bool CanCancel => Order.Status == OrderStatus.Pending;
    }
}

