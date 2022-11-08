using API.Models;
using FoxProDbExtentionConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;
using System.Data;

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

        [HttpGet("/test")]
        public IActionResult Test() 
        {
            IFoxDbContext foxDb = new FoxDbContext("Provider = VFPOLEDB.1; Data Source = c:\\basesdedatosfoxpro\\datacentro1; Collating Sequence = Machine;");
            var result0 = foxDb.Update("UPDATE ped_respedidos SET estado=103 WHERE STR(refpedido,10)=STR(170534,10)");
            var result1 = foxDb.Update("UPDATE ped_detpedidos SET estado=3 WHERE refpedido = 170534 AND refprod=10454 AND refpres = 2");
            var result2 = foxDb.Update("UPDATE ped_detpedidos SET estado=5 WHERE STR(refpedido,10)+STR(refprod,6)+STR(refpres,2)=STR(170534,10)+STR(10454,6)+STR(2,2)");
            return Ok();
        }
    }
}
