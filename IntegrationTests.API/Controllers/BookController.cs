using IntegrationTests.API.Context;
using IntegrationTests.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.API.Controllers;

[ApiController]
[Route("api/controller/books")]
public class BookController : ControllerBase
{
    private readonly BookContext _context;
    public BookController(BookContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
    public async Task<List<Book>> Get()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);

        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Book book)
    {
        if (book.Title == null)
        {
            return BadRequest("Title is required");
        }

        if (book.Author == null)
        {
            return BadRequest("Author is required");
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Created($"api/books/{book.Id}", book);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Post(int id, Book book)
    {
        var existing = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = book.Title;
        existing.Author = book.Author;

        await _context.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        _context.Books.Remove(existing);
        await _context.SaveChangesAsync();

        return Ok(existing);
    }
}
