using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Library.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;
        private readonly OrderDbContext _context;

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

            if (order is not null)
            {
                order.Status = OrderStatus.Complete;
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
