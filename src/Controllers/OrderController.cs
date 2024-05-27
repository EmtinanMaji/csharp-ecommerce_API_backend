using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.EntityFramework;
using api.Models;
using api.Services;
using api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(AppDbContext appDbContext)
        {
            _orderService = new OrderService(appDbContext);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] QueryParameters queryParams)
        {
            try
            {
                var orders = await _orderService.GetAllOrders(queryParams);
                
                if (orders == null)
                {
                    return ApiResponse.NotFound("There is no orders to display");
                }
                return ApiResponse.Success(orders, "Orders are returned successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// Get By ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            try
            {

                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound(new ErrorResponse { Message = $"There is no order found with ID : {orderId}" });
                }
                else
                {
                    return Ok(new SuccessResponse<Order>
                    {
                        Message = "Category is returned successfully",
                        Data = order
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not return the order");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }

        }

        /// Post
        [HttpPost]
        public async Task<IActionResult> PostOrder(Order newOrder)
        {
            try
            {
                var createdOrder = await _orderService.PostOrder(newOrder);
                if (createdOrder == null)
                {
                    return NotFound(new ErrorResponse { Message = $"There is no order entered !" });
                }
                else
                {
                    return Ok(new SuccessResponse<Order>
                    {
                        Message = "Order is created successfully",
                        Data = createdOrder
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not create new category");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
        /// Put
        [HttpPut("{orderId}")]
        public async Task<IActionResult> PutOrder(Guid orderId, Order putorder)
        {
            try
            {

                var order = await _orderService.PutOrder(orderId, putorder);
                if (order == null)
                {
                    return NotFound(new ErrorResponse { Message = "There is no order found to update." });
                }
                else
                {
                    return Ok(new SuccessResponse<Order>
                    {
                        Message = "Order is updated  succeefully",
                        Data = order
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not update the order");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
        /// DElete
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            try
            {

                var result = await _orderService.DeleteOrder(orderId);
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"The order with ID : {orderId} is not found to be deleted" });
                }
                else
                {
                    return Ok(new SuccessResponse<Order>
                    {
                        Message = "Order is deleted succeefully",
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not delete the order");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }

        }
    }
}

/* 
 [HttpPost]
        public IActionResult PostOrder(OrderModel newOrder)
        {
            try
            {
                _orderService.PostOrder(newOrder);
                return Ok("Order Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{orderId}")]
        public IActionResult PutOrder(Guid orderId, OrderModel putorder)
        {
            try
            {
                _orderService.PutOrder(orderId, putorder);
                return Ok("Order Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{orderId}")]
        public IActionResult DeleteOrder(Guid orderId)
        {
            try
            {
                _orderService.DeleteOrder(orderId);
                return Ok("Order Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
*/