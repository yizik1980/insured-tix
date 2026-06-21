namespace BookstoreAPI.Models;

public class Book
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleLang { get; set; } = "en";
    public List<string> Authors { get; set; } = [];
    public string Category { get; set; } = string.Empty;
    public string? Cover { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
}
