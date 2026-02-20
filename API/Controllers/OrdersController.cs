using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        private readonly API.Services.EmailService _emailService = new API.Services.EmailService();
        public OrdersController(IOrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = orderDto.ShipToAddress.ZipCode;//HttpContext.User.RetrieveEmailFromPrincipal();
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);
            var order = await _orderService.CreateOrderAsync(email, 
                orderDto.DeliveryMethodId, 
                orderDto.BasketId,
                address);

            if (order == null) return BadRequest(new ApiResponse(400, "Problem creating order") );

            // Send email to admin
            var adminEmail = "rami13195@gmail.com";
            var subject = $"New Order #{order.Id}";
            var body = $"Order Details:\n" +
                $"Buyer: {order.BuyerEmail}\n" +
                $"Date: {order.OrderDate}\n" +
                $"Address: {order.ShipToAddress.Street}, {order.ShipToAddress.City}, {order.ShipToAddress.Country}, {order.ShipToAddress.ZipCode}\n" +
                $"Items:\n" +
                string.Join("\n", order.OrderItems.Select(i => $"- {i.ItemOrdered.ProductName} x{i.Quantity} @ {i.Price}")) +
                $"\nSubtotal: {order.Subtotal}\nTotal: {order.GetTotal()}";
            await _emailService.SendOrderEmailAsync(adminEmail, subject, body);

            return Ok(order);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var orders = await _orderService.GetOrdersForUserAsync(email);
            return Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var order = await _orderService.GetOrderByIdAsync(id,email);
            if (order == null) return NotFound(new ApiResponse(404));
            return _mapper.Map<OrderToReturnDto>(order);
        }

        [HttpGet("deliverymethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodsAsync());
        }
    }
}
