using BookstoreAPI.Models;

namespace BookstoreAPI.Services;

public interface IBookService
{
    IEnumerable<Book> GetAll();
    Book? GetByIsbn(string isbn);
    void Add(Book book);
    bool Update(string isbn, Book book);
    bool Delete(string isbn);
    string GenerateHtmlReport();
}
