using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Library.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;
        private readonly OrderDbContext _context;

        public PaymentFailedEventConsumer(ILogger<PaymentCompletedEventConsumer> logger, OrderDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

            if (order is not null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.Status}");
            }
            else
            {
                _logger.LogError($"Order (Id={context.Message.OrderId}) not found.");
            }
        }
    }
}
