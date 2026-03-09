using Microsoft.AspNetCore.Mvc;
using SimpleStore.DTOs;
using SimpleStore.Repositories;

namespace SimpleStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderRepository repo) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] PlaceOrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
            return BadRequest(new { message = "Items are required." });
        var (header, items) = await repo.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = header.OrderId }, new { header, items });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        var result = await repo.GetByIdAsync(id);
        return result is null ? NotFound(new { message = $"Order {id} not found." }) : Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await repo.ListPagedAsync(pageNumber, pageSize);
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(items);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateReplaceItems(int id, [FromBody] UpdateOrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
            return BadRequest(new { message = "Items are required." });
        var (header, items) = await repo.UpdateReplaceItemsAsync(id, request);
        return Ok(new { header, items });
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await repo.DeleteAsync(id);
        return ok ? NoContent() : NotFound(new { message = $"Order {id} not found." });
    }


}