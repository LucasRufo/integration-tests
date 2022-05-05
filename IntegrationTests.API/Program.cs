using IntegrationTests.API.Context;
using IntegrationTests.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Data Source=Books.db";

builder.Services.AddSqlite<BookContext>(connectionString);

var app = builder.Build();

await EnsureDb(app.Services, app.Logger);

app.MapGet("/api/books", async (BookContext context) =>
{
    var books = await context.Books.ToListAsync();

    return books;
});

app.MapGet("/api/books/{id}", async (int id, BookContext context) =>
{
    var book = await context.Books.FirstOrDefaultAsync(m => m.Id == id);

    return book == null ? Results.NotFound() : Results.Ok(book);
})
    .Produces<Book>(200)
    .Produces(404);

app.MapPost("/api/books", async (Book book, BookContext context) =>
{
    if (book.Title == null)
    {
        return Results.BadRequest("Title is required");
    }

    if (book.Author == null)
    {
        return Results.BadRequest("Author is required");
    }

    context.Books.Add(book);
    await context.SaveChangesAsync();

    return Results.Created($"api/books/{book.Id}", book);
})
    .Produces<Book>(201)
    .Produces(400);

app.MapPut("/api/books/{id}", async (int id, Book book, BookContext context) =>
{
    var existing = await context.Books.FirstOrDefaultAsync(m => m.Id == id);

    if (existing == null)
    {
        return Results.NotFound();
    }

    existing.Title = book.Title;
    existing.Author = book.Author;

    await context.SaveChangesAsync();

    return Results.Ok(existing);
})
    .Produces<Book>(200)
    .Produces(404);

app.MapDelete("api/books/{id}", async (int id, BookContext context) =>
{
    var existing = await context.Books.FirstOrDefaultAsync(m => m.Id == id);

    if (existing == null)
    {
        return Results.NotFound();
    }

    context.Books.Remove(existing);
    await context.SaveChangesAsync();

    return Results.Ok(existing);
})
    .Produces<Book>(200)
    .Produces(404); ;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();


async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("Ensuring database" + " '{connectionString}'", connectionString);

    using var db = services.CreateScope().ServiceProvider.GetRequiredService<BookContext>();
    await db.Database.EnsureCreatedAsync();

    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
}

public partial class Program { }


