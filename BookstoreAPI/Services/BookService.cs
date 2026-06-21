using System.Xml.Linq;
using BookstoreAPI.Models;

namespace BookstoreAPI.Services;

public class BookService : IBookService
{
    private readonly string _xmlPath;
    private readonly object _lock = new();

    public BookService(IConfiguration configuration)
    {
        _xmlPath = configuration["BookstoreXmlPath"]
            ?? throw new InvalidOperationException("BookstoreXmlPath is not configured.");
    }

    public IEnumerable<Book> GetAll()
    {
        var doc = LoadDocument();
        return doc.Root!.Elements("book").Select(MapToBook);
    }

    public Book? GetByIsbn(string isbn)
    {
        var doc = LoadDocument();
        var element = FindByIsbn(doc, isbn);
        return element is null ? null : MapToBook(element);
    }

    public void Add(Book book)
    {
        lock (_lock)
        {
            var doc = LoadDocument();

            if (FindByIsbn(doc, book.Isbn) is not null)
                throw new InvalidOperationException($"Book with ISBN {book.Isbn} already exists.");

            doc.Root!.Add(MapToElement(book));
            SaveDocument(doc);
        }
    }

    public bool Update(string isbn, Book book)
    {
        lock (_lock)
        {
            var doc = LoadDocument();
            var element = FindByIsbn(doc, isbn);
            if (element is null) return false;

            element.ReplaceWith(MapToElement(book));
            SaveDocument(doc);
            return true;
        }
    }

    public bool Delete(string isbn)
    {
        lock (_lock)
        {
            var doc = LoadDocument();
            var element = FindByIsbn(doc, isbn);
            if (element is null) return false;

            element.Remove();
            SaveDocument(doc);
            return true;
        }
    }

    public string GenerateHtmlReport()
    {
        var books = GetAll().ToList();
        var rows = string.Join("\n", books.Select(b =>
            $"<tr>" +
            $"<td>{System.Web.HttpUtility.HtmlEncode(b.Title)}</td>" +
            $"<td>{System.Web.HttpUtility.HtmlEncode(string.Join(", ", b.Authors))}</td>" +
            $"<td>{System.Web.HttpUtility.HtmlEncode(b.Category)}</td>" +
            $"<td>{b.Year}</td>" +
            $"<td>{b.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}</td>" +
            "</tr>"));

        var generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        return "<!DOCTYPE html>\n" +
               "<html lang=\"he\" dir=\"rtl\">\n" +
               "<head>\n" +
               "  <meta charset=\"UTF-8\"/>\n" +
               "  <title>Bookstore Report</title>\n" +
               "  <style>\n" +
               "    body { font-family: Arial, sans-serif; padding: 24px; }\n" +
               "    h1 { color: #333; }\n" +
               "    table { border-collapse: collapse; width: 100%; }\n" +
               "    th, td { border: 1px solid #ccc; padding: 8px 12px; text-align: left; }\n" +
               "    th { background-color: #4a7bb5; color: white; }\n" +
               "    tr:nth-child(even) { background-color: #f2f2f2; }\n" +
               "  </style>\n" +
               "</head>\n" +
               "<body>\n" +
               "  <h1>Bookstore Report</h1>\n" +
               $"  <p>Generated: {generated}</p>\n" +
               "  <table>\n" +
               "    <thead><tr><th>Title</th><th>Author(s)</th><th>Category</th><th>Year</th><th>Price</th></tr></thead>\n" +
               $"    <tbody>\n{rows}\n    </tbody>\n" +
               "  </table>\n" +
               "</body>\n" +
               "</html>";
    }

    private XDocument LoadDocument() => XDocument.Load(_xmlPath);

    private void SaveDocument(XDocument doc) =>
        doc.Save(_xmlPath, SaveOptions.None);

    private static XElement? FindByIsbn(XDocument doc, string isbn) =>
        doc.Root!.Elements("book")
            .FirstOrDefault(e => e.Element("isbn")?.Value == isbn);

    private static Book MapToBook(XElement e) => new()
    {
        Isbn = e.Element("isbn")?.Value ?? string.Empty,
        Title = e.Element("title")?.Value ?? string.Empty,
        TitleLang = e.Element("title")?.Attribute("lang")?.Value ?? "en",
        Authors = e.Elements("author").Select(a => a.Value).ToList(),
        Category = e.Attribute("category")?.Value ?? string.Empty,
        Cover = e.Attribute("cover")?.Value,
        Year = int.TryParse(e.Element("year")?.Value, out var y) ? y : 0,
        Price = decimal.TryParse(e.Element("price")?.Value,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var p) ? p : 0m
    };

    private static XElement MapToElement(Book book)
    {
        var el = new XElement("book",
            new XAttribute("category", book.Category));

        if (!string.IsNullOrEmpty(book.Cover))
            el.Add(new XAttribute("cover", book.Cover));

        el.Add(new XElement("isbn", book.Isbn));
        el.Add(new XElement("title",
            new XAttribute("lang", book.TitleLang),
            book.Title));

        foreach (var author in book.Authors)
            el.Add(new XElement("author", author));

        el.Add(new XElement("year", book.Year));
        el.Add(new XElement("price",
            book.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)));

        return el;
    }
}
