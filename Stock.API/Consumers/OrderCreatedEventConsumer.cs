using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Library.Events;
using Shared.Library.Settings;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    /// <summary>
    /// Burada rabbitmq'ya publish yerine send methodu kullandık çünkü benim bu eventi dinleyen sadece bir servisim var oda ödeme servisi.
    /// StockNotReservedEvent'ini publish olarak göndericem çünkü bu eventi dinleyen birden fazla microservis olabilir.
    /// Publish te kuyruk belirtmene gerek yok Send methodunu kullanırsan kuyruk belirtmen gerekiyor.
    /// Burada biraz farklı kullanımları görmek adına bu şekilde yaptık.
    /// </summary>
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly StockDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(StockDbContext dbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await _dbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Count -= item.Count;

                        await _dbContext.SaveChangesAsync();
                    }
                }

                _logger.LogInformation($"Stock was reserved for Buyer Id: {context.Message.BuyerId}");

                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StockReservedEventQueueName}"));

                StockReservedEvent stockReservedEvent = new StockReservedEvent
                {
                    Payment = context.Message.Payment,
                    OrderItems = context.Message.OrderItems,
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId
                };

                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                _logger.LogInformation($"Not enough stock for Buyer Id: {context.Message.BuyerId}");
                
                await _publishEndpoint.Publish(new StockNotReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    Message = "Not enough stock"
                });
            }
        }
    }
}
