﻿using Shared.Library.Messages;

namespace Shared.Library.Events
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
