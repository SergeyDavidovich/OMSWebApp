using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWebApp.Server.AppDBContext;
using OMSWebApp.Shared.Models;
using OMSWebApp.Shared.StatisticsObjects;

namespace OMSWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public StatisticsController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: api/Statistics
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersByCountry>>> GetOrdersByCountries()
        {
            var groupedOrders = _context.Orders.GroupBy(order => order.Customer.Country);

            var ordersByCountries = await groupedOrders.Select(orderGroup => new OrdersByCountry { CountryName = orderGroup.Key, OrdersCount = orderGroup.Count() }).
                OrderByDescending(ordersByCountry => ordersByCountry.OrdersCount).
                Take(10).
                ToListAsync();

            return ordersByCountries;
        }



        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

       
    }
}
