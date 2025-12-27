using BasicWebApplicationCsharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApplicationCsharp.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
            => _orderService = orderService;

        [Authorize(Roles = "Manager")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] OrderDto request)
        {
            var order = _orderService.Create(request.UserId);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [Authorize]
        [HttpPost("{id:int}/items")]
        public IActionResult AddOrderItem(int id, [FromBody] OrderItemFullDto request)
        {
            var order = _orderService.AddItem(id, request.ProductId, request.Quantity);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize]
        [HttpPut("{id:int}/items")]
        public IActionResult ChangeOrderItemQuantity(int id, [FromBody] OrderItemFullDto request)
        {
            var order = _orderService.ChangeItemQuantity(id, request.ProductId, request.Quantity);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize]
        [HttpDelete("{id:int}/items")]
        public IActionResult DeleteOrderItem(int id, [FromBody] OrderItemIdDto request)
        {
            var order = _orderService.RemoveItem(id, request.ProductId);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize]
        [HttpPost("{id:int}/place")]
        public IActionResult Place(int id)
        {
            var order = _orderService.Place(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // This is a fake payment endpoint for demonstration purposes. Immediately marks the order as paid.
        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/pay")]
        public IActionResult Pay(int id)
        {
            var order = _orderService.Pay(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/confirm-pickup")]
        public IActionResult ConfirmPickup(int id)
        {
            var order = _orderService.ConfirmPickup(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/confirm-delivery")]
        public IActionResult ConfirmDelivery(int id)
        {
            var order = _orderService.ConfirmDelivery(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/complete")]
        public IActionResult Complete(int id)
        {
            var order = _orderService.Complete(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("{id:int}/cancel")]
        public IActionResult Cancel(int id)
        {
            var order = _orderService.Cancel(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        public class OrderDto
        {
            public int UserId { get; set; }
        }

        public class OrderItemFullDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class OrderItemIdDto
        {
            public int ProductId { get; set; }
        }
    }
}
