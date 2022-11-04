using API.Models;
using FoxProDbExtentionConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IFoxDbContext _foxDbContext;

        public OrdersController(IFoxDbContext foxDbContext)
        {
            _foxDbContext = foxDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _foxDbContext.GetListAsync<Orders>("SELECT * FROM orders");
            var result2 = _foxDbContext.GetListAsyncJson("SELECT * FROM orders");
            return Ok(result2) ;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var result = await _foxDbContext.GetFirstAsync<Orders>($"SELECT * FROM orders where orderId={orderId}");
            return Ok(result);
        }
    }
}
