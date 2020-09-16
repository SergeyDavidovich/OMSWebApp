using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWebApp.Server.Data;
using OMSWebApp.Shared.Models;
using OMSWebApp.Shared.StatisticsObjects;

namespace OMSWebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public StatisticsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Statistics/GetOrdersByCountries
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

        // GET: api/Statistics/GetSalesByCategories
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesByCategory>>> GetSalesByCategories()
        {
            var groupedOrderDetail = _context.OrderDetails.GroupBy(od => od.Product.Category.CategoryName);

            var salesByCategories = await groupedOrderDetail.Select(orderDetailGroup => new SalesByCategory
            {
                CategoryName = orderDetailGroup.Key,
                SalesSum = orderDetailGroup.Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice)
            }).OrderByDescending(salesByCategory => salesByCategory.SalesSum).ToListAsync();

            return salesByCategories;
        }
        // GET: api/Statistics/GetSalesByEmployees
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesByEmployee>>> GetSalesByEmployees()
        {
            var employees = await _context.Employees.Include(employee => employee.Orders).
                ThenInclude(employeeOrder => employeeOrder.OrderDetails).
                ToListAsync();

            return await Task.Run(() =>
            {
                var salesByEmployees = employees.Select(employee => new SalesByEmployee
                {
                    LastName = employee.LastName,
                    SalesSum = employee.Orders.Sum(order => order.OrderDetails.Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice))
                }).ToList();

                return salesByEmployees;
            });
        }




        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
