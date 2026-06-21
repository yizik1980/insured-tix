using Microsoft.AspNetCore.Mvc;
using BookstoreAPI.Models;
using BookstoreAPI.Services;

namespace BookstoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAll() =>
        Ok(_bookService.GetAll());

    [HttpGet("{isbn}")]
    public ActionResult<Book> GetByIsbn(string isbn)
    {
        var book = _bookService.GetByIsbn(isbn);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Book book)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _bookService.Add(book);
        return CreatedAtAction(nameof(GetByIsbn), new { isbn = book.Isbn }, book);
    }

    [HttpPut("{isbn}")]
    public IActionResult Update(string isbn, [FromBody] Book book)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = _bookService.Update(isbn, book);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{isbn}")]
    public IActionResult Delete(string isbn)
    {
        var deleted = _bookService.Delete(isbn);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("report/html")]
    public ContentResult GetHtmlReport()
    {
        var html = _bookService.GenerateHtmlReport();
        return Content(html, "text/html");
    }
}
