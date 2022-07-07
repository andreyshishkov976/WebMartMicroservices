namespace WebMart.Extensions.DTOs.Basket
{
    public class BasketReadDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int ProductCount { get; set; }
        public double TotalCost { get; set; }
    }
}