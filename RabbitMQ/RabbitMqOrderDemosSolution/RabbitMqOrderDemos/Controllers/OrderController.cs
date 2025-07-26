using Microsoft.AspNetCore.Mvc;
using RabbitMqOrderDemos.Dtos;
using RabbitMqOrderDemos.Services;

namespace RabbitMqOrderDemos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MessagePublisher _publisher;

        public OrderController()
        {
            _publisher = new MessagePublisher();
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderDto order)
        {
            _publisher.Publish(order);
            return Ok(new { message = "Order received and published!" });
        }
    }
}

namespace RabbitMqOrderDemos.Dtos
{
    public class OrderDto
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }
}