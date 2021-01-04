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

        // GET: api/Statistics/GetCustomersByCountries
        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<CustomersByCountry>> GetCustomersByCountries()
        {
            var groupedCustomers = _context.Customers.GroupBy(cust => cust.Country);

            var customersByCountries = await groupedCustomers.Select(customerGroup => new CustomersByCountry
            {
                CountryName = customerGroup.Key,
                CustomerCount = customerGroup.Count()
            })
                .OrderByDescending(customersByCountries => customersByCountries.CustomerCount).Take(10).ToListAsync();

            return customersByCountries;
        }

        // GET: api/Statistics/GetProductsByCategories
        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<ProductsByCategory>> GetProductsByCategories()
        {
            var groupedProducts = _context.Products.GroupBy(prod => prod.Category.CategoryName);

            var productsByCategories = await groupedProducts.Select(productGroup => new ProductsByCategory()
            {
                CategoryName = productGroup.Key,
                ProductsCount = productGroup.Count()
            })
                .OrderByDescending(productsByCategories => productsByCategories.ProductsCount).ToListAsync();

            return productsByCategories;
        }

        //GET: api/Statistics/GetSalesByCountries
        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSalesByCountries()
        {
            var groupedDetails = _context.OrderDetails.GroupBy(o => o.Order.Customer.Country);

            var salesByCountries = await groupedDetails.Select(groupeDetail => new SalesByCountry()
            {
                CountryName = groupeDetail.Key,
                CountrySum = groupeDetail.Sum(g => g.Quantity * g.UnitPrice)
            }).OrderByDescending(salesBycountries=>salesBycountries.CountrySum).Take(10).ToListAsync();
          
            return salesByCountries;
        }

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
                }).OrderBy(salesByEmployee => salesByEmployee.SalesSum).ToList();

                return salesByEmployees;
            });
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchasesByCustomer>>> GetPurchasesByCustomers()
        {
            var customers = await _context.Customers.Include(customer => customer.Orders).
                ThenInclude(customerOrder => customerOrder.OrderDetails).
                ToListAsync();

            return await Task.Run(() =>
            {
                var purchasesByCustomers = customers.Select(customer => new PurchasesByCustomer
                {
                    CustomerName = customer.CompanyName,
                    PurchaseSum = customer.Orders.Sum(order => order.OrderDetails.Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice))
                }).OrderByDescending(purchasesByCustomer => purchasesByCustomer.PurchaseSum).Take(10).ToList();

                return purchasesByCustomers;
            });
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
