using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {

        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork)
        {

            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Core.Entities.OrderAggregate.Address shippingAddress)
        {
            // get basket from basketrepo
            var basket = await _basketRepo.GetBasketAsync(basketId);
            // we trust quentities, items, but will check the prices
            // get items from product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id,
                    productItem.Title,
                    productItem.Thumbnail);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            // get delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(1);//GetByIdAsync(deliveryMethodId)
            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // check to see if order exists
            //var order = await GetOrderByPaymentIntentIdAsync(basket.PaymentIntentId);
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod
                    , subtotal, "basket.PaymentIntentId");
            _unitOfWork.Repository<Order>().Add(order);

            //if (order != null)
            //{
            //    order.ShipToAddress = shippingAddress;
            //    order.DeliveryMethod = deliveryMethod;
            //    order.Subtotal = subtotal;
            //    _unitOfWork.Repository<Order>().Update(order);
            //}
            //else
            //{
            //    // create order
            //    order = new Order(items, buyerEmail, shippingAddress, deliveryMethod
            //        , subtotal, basket.PaymentIntentId);
            //    _unitOfWork.Repository<Order>().Add(order);
            //}

            // save to db

            var result = await _unitOfWork.Complete();
            if (result <= 0) return order; // should be return null
            // delete basket
            // will call this from client side
            //await _basketRepo.DeleteBasketAsync(basketId);



            // return order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }

        public async Task<Order> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }
    }
}
