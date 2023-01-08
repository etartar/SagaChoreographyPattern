using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Library.Events;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly ILogger<PaymentFailedEventConsumer> _logger;
        private readonly StockDbContext _context;

        public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, StockDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (stock is not null)
                {
                    stock.Count += item.Count;
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation($"Stock was released for Order Id ({context.Message.OrderId})");
        }
    }
}
