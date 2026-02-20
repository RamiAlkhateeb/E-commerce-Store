using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Specifications
{
    public class OrdersWithItemsSpecification : BaseSpecification<Order>
    {
        // 1. NEW: Parameterless constructor to get ALL orders (For the Admin Dashboard)
        public OrdersWithItemsSpecification()
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}
