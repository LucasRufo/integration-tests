using IntegrationTests.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.API.Context;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
}


