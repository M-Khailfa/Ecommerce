using Ecommerce.Core.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Dtos.Order
{
    public record OrderItemRequestDto(
        [Required] int ProductId,
        [Range(1, int.MaxValue)] int Quantity
    );

    public record PlaceOrderDto(
        [Required, MinLength(1)] List<OrderItemRequestDto> Items,
        string? ShippingAddress
    );

    public record OrderItemDto(
        int ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal
    );

    public record OrderDto(
        int Id,
        string UserId,
        OrderStatus Status,
        decimal Total,
        string? ShippingAddress,
        DateTime PlacedAt,
        List<OrderItemDto> Items
    );

    public record UpdateOrderStatusDto(
        [Required] OrderStatus Status  
    );
}
