using Core.Entities.OrderAggregate;


namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail,
            int deliveryMethod ,
            string basketId ,
            Address shippingAddress);

        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<IReadOnlyList<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();

        Task <string> DeleteOrderAsync(int id);
    }
}
