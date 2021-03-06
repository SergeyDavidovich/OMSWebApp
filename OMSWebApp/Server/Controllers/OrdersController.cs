﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.EntityFrameworkCore.Internal;
using OMSWebApp.Server.Data;
using OMSWebApp.Shared.Models;

namespace OMSWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrdersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            //var orders = await _context.Orders.ToListAsync();

            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();

            return orders;
        }

        //https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == id)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            //var orderDetails = order.OrderDetails
            //    .Select(o => new OrderDetails
            //    {
            //        OrderId = o.OrderId,
            //        ProductId = o.ProductId,
            //        UnitPrice = o.UnitPrice,
            //        Quantity = o.Quantity,
            //        Discount = o.Discount,
            //    });

            //order.OrderDetails = orderDetails.ToList();

            return order;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody] Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            var result = CreatedAtAction(
                nameof(GetOrder),
                new { Id = order.OrderId },
                order);
            return result;
        }

        // PUT: api/orders/10228
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order item)
        {
            if (id != item.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orders/10248
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var details = _context.OrderDetails.Where(o => order.OrderId == id);

                    _context.OrderDetails.RemoveRange(details);

                    _context.Orders.Remove(order);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return NoContent();
        }

        // DELETE: api/orders/
        [HttpDelete]
        public async Task<IActionResult> DeleteOrdersRange([FromBody] int[] range)
        {
            List<Order> orders = new List<Order>();
            List<OrderDetail> details = new List<OrderDetail>();

            foreach (int id in range)
            {
                var order = await _context.Orders.FindAsync(id);
                if (order != null) orders.Add(order);
            }

            if (orders.Count == 0) return NotFound();

            foreach (var item in orders)
            {
                var detail = _context.OrderDetails.Where(o => o.OrderId == item.OrderId) as OrderDetail;
                if (detail != null) details.Add(detail);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (details.Count != 0) _context.OrderDetails.RemoveRange(details);

                    _context.Orders.RemoveRange(orders);
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}




