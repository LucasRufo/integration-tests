using IntegrationTests.API.Context;
using IntegrationTests.API.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace IntegrationTests.Tests.MinimalApi;

public class BooksApiFixture : IDisposable
{
    public BooksApiApplication Api { get; private set; }
    public HttpClient Client { get; private set; }

    public BooksApiFixture()
    {
        Api = new BooksApiApplication();

        using var scope = Api.Services.CreateScope();
        
        var provider = scope.ServiceProvider;
        
        using var notesDbContext = provider.GetRequiredService<BookContext>();
        
        notesDbContext.Database.EnsureCreated();

        notesDbContext.Books.Add(new Book { Id = 1, Title = "First Book", Author = "First Author" });
        notesDbContext.Books.Add(new Book { Id = 2, Title = "Second Book", Author = "Second Author" });
        notesDbContext.Books.Add(new Book { Id = 3, Title = "Third Book", Author = "Third Author" });
        notesDbContext.Books.Add(new Book { Id = 4, Title = "Fourth Book", Author = "Fourth Author" });
        notesDbContext.Books.Add(new Book { Id = 5, Title = "Fifth Book", Author = "Fifth Author" });
        notesDbContext.SaveChanges();

        Client = Api.CreateClient();
    }

    public void Dispose()
    {
        Api.Dispose();
        GC.SuppressFinalize(this);
    }
}
