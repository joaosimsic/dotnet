using Microsoft.AspNetCore.Mvc;
using PhoneBook.Api.DTOs;
using PhoneBook.Api.Services;

namespace PhoneBook.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ContactsController(IContactService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ContactDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var result = await service.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> GetById(int id)
    {
        var contact = await service.GetByIdAsync(id);
        if (contact == null)
            return NotFound(new { error = "Contact not found", id });
        return Ok(contact);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResultDto<ContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ContactDto>>> Search(
        [FromQuery] string q = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var result = await service.SearchAsync(q, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactDto dto)
    {
        var contact = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactDto>> Update(int id, [FromBody] UpdateContactDto dto)
    {
        var contact = await service.UpdateAsync(id, dto);
        if (contact == null)
            return NotFound(new { error = "Contact not found", id });
        return Ok(contact);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> Delete(int id)
    {
        var contact = await service.DeleteAsync(id);
        if (contact == null)
            return NotFound(new { error = "Contact not found", id });
        return Ok(contact);
    }
}
