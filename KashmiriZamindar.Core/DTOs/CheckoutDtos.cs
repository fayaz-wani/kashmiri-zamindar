using System.ComponentModel.DataAnnotations;

namespace KashmiriZamindar.Core.Dtos
{
    public class CheckoutRequestDto
    {
        public ContactInfoDto ContactInfo { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public ShippingMethodDto ShippingMethod { get; set; }
        public PaymentInfoDto PaymentInfo { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        public AdditionalFieldsDto AdditionalFields { get; set; }
    }

    public class ContactInfoDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool SubscribeMarketing { get; set; }
        public string OrderNotes { get; set; }
    }

    public class ShippingAddressDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
    }

    public class ShippingMethodDto
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string EstimatedDelivery { get; set; }
    }


        public class PaymentInfoDto
        {
            [Required]
            public string PaymentType { get; set; }  // COD, Online, Card

            // Make these nullable and remove Required attribute
            public string? CardNumber { get; set; }
            public string? CardExpiry { get; set; }
            public string? CardCVV { get; set; }
            public string? NameOnCard { get; set; }

            public BillingAddressDto BillingAddress { get; set; }
        }
 
    public class BillingAddressDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
    }



    public class AdditionalFieldsDto
    {
        public string? GstNumber { get; set; }
        public string? DeliveryInstructions { get; set; }
        public string? AlternatePhone { get; set; }
        public string? PreferredDeliveryTime { get; set; }
    }
}