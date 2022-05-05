using FluentAssertions;
using IntegrationTests.API.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Tests.MinimalApi;

public class BooksIntegrationTests : IClassFixture<BooksApiFixture>
{
    private readonly BooksApiFixture _fixture;
    public BooksIntegrationTests(BooksApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Get_ShouldReturn_Books()
    {
        var client = _fixture.Client;

        var books = await client.GetFromJsonAsync<List<Book>>("/api/books");

        books.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetById_ShouldReturn_Book()
    {
        var client = _fixture.Client;

        var book = await client.GetFromJsonAsync<Book>("/api/books/5");

        var expected = new Book
        {
            Id = 5,
            Title = "Fifth Book",
            Author = "Fifth Author",
        };

        book.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldReturn_NotFound()
    {
        var client = _fixture.Client;

        var response = await client.GetAsync("/api/books/10");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturn_CreatedBook()
    {
        var client = _fixture.Client;

        var response = await client.PostAsJsonAsync("/api/books", new Book { Title = "New Book", Author = "New Author" });

        var book = await response.Content.ReadFromJsonAsync<Book>();

        book.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_ShouldReturn_BadRequestWhenTitleIsNull()
    {
        var client = _fixture.Client;

        var response = await client.PostAsJsonAsync("/api/books", new Book { Author = "New Author" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ShouldReturn_BadRequestWhenAuthorIsNull()
    {
        var client = _fixture.Client;

        var response = await client.PostAsJsonAsync("/api/books", new Book { Title = "New Book" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_ShouldReturn_UpdatedBook()
    {
        var client = _fixture.Client;

        var addedBookResponse = await client.PostAsJsonAsync("/api/books", new Book { Title = "New Book", Author = "New Author" });

        var addedBook = await addedBookResponse.Content.ReadFromJsonAsync<Book>();

        var response = await client.PutAsJsonAsync($"/api/books/{addedBook?.Id}", new Book { Id = addedBook.Id, Title = "Updated Book", Author = "Updated Author" });

        var book = await response.Content.ReadFromJsonAsync<Book>();

        var expected = new Book
        {
            Id = addedBook.Id,
            Title = "Updated Book",
            Author = "Updated Author",
        };

        book.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Put_ShouldReturn_NotFound()
    {
        var client = _fixture.Client;

        var response = await client.PutAsJsonAsync("/api/books/10", new Book { Id = 10, Title = "New Book", Author = "New Author" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturn_Ok()
    {
        var client = _fixture.Client;

        var addedBookResponse = await client.PostAsJsonAsync("/api/books", new Book { Title = "New Book", Author = "New Author" });

        var addedBook = await addedBookResponse.Content.ReadFromJsonAsync<Book>();

        var response = await client.DeleteAsync($"/api/books/{addedBook?.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturn_NotFound()
    {
        var client = _fixture.Client;

        var response = await client.DeleteAsync("/api/books/10");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
