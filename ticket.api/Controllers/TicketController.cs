using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket.api.Model;
using ticket.api.Service;

namespace ticket.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class TicketController : ControllerBase
{
    private readonly TicketService _ticketService;

    public TicketController(TicketService ticketService)
        => _ticketService = ticketService;

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TicketItemDto>>> ListTickets()
    {
        var tickets = await _ticketService.GetTicketList();
        return Ok(tickets);
    }

    [HttpGet("{id:long}", Name = "GetTicket")]
    [Produces("application/json")]
    [ProducesResponseType<TicketDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDetailsDto>> GetTicket([FromRoute] long id)
    {
        var ticket = await _ticketService.GetTicketDetails(id);
        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType<TicketDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateTicket([FromBody] TicketCreateDto ticketCreateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ticket = await _ticketService.SaveTicket(ticketCreateDto);
        return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:long}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EditTicket([FromRoute] long id, [FromBody] TicketUpdateDto ticketUpdateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _ticketService.UpdateTicket(ticketUpdateDto, id);
        return result == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("{id:long}")]
    [Produces("text/plain")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> RemoveTicket([FromRoute] long id)
    {
        var result = await _ticketService.RemoveTicket(id);
        return result == 0 ? NotFound() : Accepted("Ticket deletion in progress...");
    }
}
