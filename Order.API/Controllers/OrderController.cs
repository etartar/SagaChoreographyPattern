using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Shared.Library.Events;
using Shared.Library.Messages;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto orderCreate)
        {
            var newOrder = Models.Order.Create(orderCreate.BuyerId)
                .AddAddress(orderCreate.Address.Line, orderCreate.Address.Province, orderCreate.Address.District);

            orderCreate.OrderItems.ForEach(item => newOrder.AddOrderItem(item.ProductId, item.Price, item.Count));

            await _context.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent
            {
                BuyerId = orderCreate.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage(orderCreate.Payment.CardName, orderCreate.Payment.CardNumber, orderCreate.Payment.Expiration, orderCreate.Payment.CVV, orderCreate.OrderItems.Sum(x => x.Price * x.Count))
            };

            orderCreate.OrderItems.ForEach(item =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage(item.ProductId, item.Count));
            });

            // exchange'e gider. Eğer exchange'e giden bir mesaja subscribe olan kuyruk yok ise bu mesaj boşa gider.
            // publish yerine send ile yaparsan mesajı kuyruğa ekler mesaj yok olmaz. Send methodu direk olarak kuyruğa gönderir.
            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
