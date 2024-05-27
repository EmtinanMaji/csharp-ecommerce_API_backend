using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.EntityFramework;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class OrderService
    {
        private AppDbContext _appDbContext;
        public OrderService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        ////// Get All
        
        // public async Task<IEnumerable<Order>> GetAllOrders()
        // {
        //     return await _appDbContext.Orders.ToListAsync();
        // }
        public async Task<PaginationDto<Order>> GetAllOrders(QueryParameters queryParams)
        {
            // Start with a base query
            var query = _appDbContext.Orders.Include(u => u.User).AsQueryable();

            // Apply search keyword filter
            if (!string.IsNullOrEmpty(queryParams.SearchKeyword))
            {
                query = query.Where(p => p.OrderStatus.ToLower().Contains(queryParams.SearchKeyword.ToLower()));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortOrder == "desc"
                ? query.OrderByDescending(u => EF.Property<object>(u, queryParams.SortBy))
                : query.OrderBy(u => EF.Property<object>(u, queryParams.SortBy));
            }

            var totalOrderCount = await query.CountAsync();

            var Orders = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();
            return new PaginationDto<Order>
            {
                Items = Orders,
                TotalCount = totalOrderCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        //////// Get By ID
        public async Task<Order?> GetOrderById(Guid orderId)
        {
            return await _appDbContext.Orders.FirstOrDefaultAsync(order => order.OrderId == orderId);
        }

        ////// Post
        public async Task<Order> PostOrder(Order newOrder)
        {
            newOrder.OrderId = Guid.NewGuid();
            newOrder.OrderDate = DateTime.UtcNow; 
            _appDbContext.Orders.Add(newOrder); // add record to context
            await _appDbContext.SaveChangesAsync(); // save to DB
            return newOrder;
        }

        //////// Update
        public async Task<Order?> PutOrder(Guid orderId, Order putOrder)
        {
            var existingOrder = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (existingOrder != null)
            {
                existingOrder.OrderStatus = putOrder.OrderStatus;
                existingOrder.OrderTotal = putOrder.OrderTotal;
            }
            await _appDbContext.SaveChangesAsync();
            return existingOrder;
        }

        /////// Delete
        public async Task<bool> DeleteOrder(Guid orderId)
        {
            var OrderToRemove = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (OrderToRemove != null)
            {
                _appDbContext.Orders.Remove(OrderToRemove);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

/* 
public IEnumerable<OrderModel> GetAllOrders()
        {
            var orders = _appDbContext.Orders
                .Select(row => new OrderModel
                {
                    OrderId = row.OrderId,
                    UserId = row.UserId,
                    OrderStatus = row.OrderStatus,
                    OrderTotal = row.OrderTotal,
                    OrderDate = row.OrderDate
                })
                .ToList();

            return orders;
        }
        public void PostOrder(OrderModel newOrder)
        {
            var order = new Order
            { // create the record
                OrderId = Guid.NewGuid(),
                UserId = newOrder.UserId,
                OrderStatus = newOrder.OrderStatus,
                OrderTotal = newOrder.OrderTotal,
                OrderDate = DateTime.Now,
            };
            _appDbContext.Orders.Add(order); // add record to context
            _appDbContext.SaveChanges(); // save to DB
        }
        public void PutOrder(Guid orderId, OrderModel putorder)
        {
            var order = _appDbContext.Orders.FirstOrDefault(order =>
            order.OrderId == orderId);
            if (order != null)
            {
                order.OrderStatus = putorder.OrderStatus;
                order.OrderTotal = putorder.OrderTotal;
                _appDbContext.SaveChanges();
            }
        }
        public void DeleteOrder(Guid orderId)
        {
            var order = _appDbContext.Orders.FirstOrDefault(order =>
            order.OrderId == orderId);
            if (order != null)
            {
                _appDbContext.Orders.Remove(order);
                _appDbContext.SaveChanges();
            }
        }
*/