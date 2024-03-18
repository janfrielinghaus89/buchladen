using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public double Price { get; set; }

    public Book(string title, string author, string genre, double price)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Genre = genre ?? throw new ArgumentNullException(nameof(genre));
        Price = price;
    }

    public void DisplayDetails()
    {
        Console.WriteLine($"Title: {Title}");
        Console.WriteLine($"Author: {Author}");
        Console.WriteLine($"Genre: {Genre}");
        Console.WriteLine($"Price: {Price:C}");
        Console.WriteLine();
    }
}

public class BookStore
{
    private List<Book> books;

    public BookStore()
    {
        books = new List<Book>();
    }

    public void AddBook(Book book)
    {
        books.Add(book);
        Console.WriteLine($"Das Buch \"{book.Title}\" wurde dem Buchladen hinzugefügt.");
        SaveBooksToJson();
    }

    // Speichern der Bücher in JSON
    private void SaveBooksToJson()
    {
        string json = JsonSerializer.Serialize(books);
        File.WriteAllText("books.json", json);
    }

    public void RemoveBook(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        if (books.Remove(book))
        {
            Console.WriteLine($"Das Buch \"{book.Title}\" wurde erfolgreich entfernt.");
        }
        else
        {
            Console.WriteLine($"Das Buch \"{book.Title}\" wurde nicht gefunden.");
        }
    }

    public List<Book> SearchBooks(string searchTerm)
    {
        if (searchTerm == null)
        {
            throw new ArgumentNullException(nameof(searchTerm));
        }

        List<Book> searchResults = new List<Book>();

        foreach (Book book in books)
        {
            if (book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                book.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                searchResults.Add(book);
            }
        }

        return searchResults;
    }

}

public class ShoppingCart
{
    private List<Book> booksInCart;

    public ShoppingCart()
    {
        booksInCart = new List<Book>();
    }

    public void AddToCart(Book book)
    {
        booksInCart.Add(book ?? throw new ArgumentNullException(nameof(book)));
        Console.WriteLine($"Das Buch \"{book.Title}\" wurde dem Warenkorb hinzugefügt.");
    }

    public void RemoveFromCart(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        if (booksInCart.Remove(book))
        {
            Console.WriteLine($"Das Buch \"{book.Title}\" wurde erfolgreich aus dem Warenkorb entfernt.");
        }
        else
        {
            Console.WriteLine($"Das Buch \"{book.Title}\" wurde nicht im Warenkorb gefunden.");
        }
    }

    public double CalculateTotalPrice()
    {
        return booksInCart.Sum(book => book.Price);
    }

    public IEnumerable<Book> GetBooksInCart()
    {
        return booksInCart.AsReadOnly();
    }
}

public partial class Program
{
    static void Main(string[] args)
    {
        BookStore bookStore = new BookStore();
        LoadBooksFromJson(bookStore); // Bücher beim Programmstart laden
        ShoppingCart shoppingCart = new ShoppingCart();

        DisplayMainMenu(bookStore, shoppingCart);
    }

    private static void LoadBooksFromJson(BookStore bookStore)
    {
        if (File.Exists("books.json"))
        {
            string json = File.ReadAllText("books.json");
            bookStore = JsonSerializer.Deserialize<BookStore>(json);
        }
    }

    public static void DisplayMainMenu(BookStore bookStore, ShoppingCart shoppingCart)
    {
        Console.WriteLine("Willkommen im Buchladen!");
        Console.WriteLine("1. Buch hinzufügen");
        Console.WriteLine("2. Bücher anzeigen");
        Console.WriteLine("3. Artikel in den Warenkorb legen");
        Console.WriteLine("4. Bücher suchen");
        Console.WriteLine("5. Warenkorb anzeigen");
        Console.WriteLine("6. Beenden");
        Console.Write("Wähle eine Option: ");

        string userInput = Console.ReadLine();

        switch (userInput)
        {
            case "1":
                AddBookManually(bookStore);
                break;
            case "2":
                DisplayBooks(bookStore);
                break;
            case "3":
                AddToCartManually(bookStore, shoppingCart);
                break;
            case "4":
                SearchBooks(bookStore);
                break;
            case "5":
                DisplayShoppingCart(shoppingCart);
                break;
            case "6":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Ungültige Option! Bitte wähle erneut.");
                DisplayMainMenu(bookStore, shoppingCart);
                break;
        }
    }

    public static void AddBookManually(BookStore bookStore)
    {
        Console.WriteLine("Bitte gib die Informationen für das neue Buch ein:");
        Console.Write("Titel: ");
        string title = Console.ReadLine();
        Console.Write("Autor: ");
        string author = Console.ReadLine();
        Console.Write("Genre: ");
        string genre = Console.ReadLine();
        Console.Write("Preis: ");
        double price = double.Parse(Console.ReadLine());

        Book newBook = new Book(title, author, genre, price);
        bookStore.AddBook(newBook);

        Console.WriteLine($"Das Buch \"{title}\" wurde erfolgreich hinzugefügt.");

        // Nach dem Hinzufügen zum Buchladen zurück zum Hauptmenü gehen
        DisplayMainMenu(bookStore, null);
    }

    public static void DisplayBooks(BookStore bookStore)
    {
        Console.WriteLine("Alle Bücher im Buchladen:");
        foreach (var book in bookStore.SearchBooks(""))
        {
            book.DisplayDetails();
        }

        // Nach dem Anzeigen der Bücher zum Hauptmenü zurückkehren
        DisplayMainMenu(bookStore, null);
    }

    public static void AddToCartManually(BookStore bookStore, ShoppingCart shoppingCart)
    {
        Console.WriteLine("Bitte gib den Titel des Buches ein, das du dem Warenkorb hinzufügen möchtest:");
        string title = Console.ReadLine();

        var searchResults = bookStore.SearchBooks(title);
        if (searchResults.Count > 0)
        {
            Book bookToAdd = searchResults[0]; // Nur das erste Suchergebnis hinzufügen
            shoppingCart.AddToCart(bookToAdd);
        }
        else
        {
            Console.WriteLine("Das Buch wurde nicht gefunden.");
        }

        // Nach dem Hinzufügen zum Warenkorb zurück zum Hauptmenü gehen
        DisplayMainMenu(bookStore, shoppingCart);
    }

    public static void SearchBooks(BookStore bookStore)
    {
        Console.WriteLine("Bitte gib den Suchbegriff ein:");
        string searchTerm = Console.ReadLine();

        var searchResults = bookStore.SearchBooks(searchTerm);
        if (searchResults.Count > 0)
        {
            Console.WriteLine("Suchergebnisse:");
            foreach (var book in searchResults)
            {
                book.DisplayDetails();
            }
        }
        else
        {
            Console.WriteLine("Keine Bücher gefunden, die zum Suchbegriff passen.");
        }

        // Nach der Suche zum Hauptmenü zurückkehren
        DisplayMainMenu(bookStore, null);
    }

    public static void DisplayShoppingCart(ShoppingCart shoppingCart)
    {
        Console.WriteLine("Artikel im Warenkorb:");
        foreach (var book in shoppingCart.GetBooksInCart())
        {
            book.DisplayDetails();
        }

        double totalPrice = shoppingCart.CalculateTotalPrice();
        Console.WriteLine($"Gesamtpreis des Warenkorbs: {totalPrice:C}");

        // Nach dem Anzeigen des Warenkorbs zum Hauptmenü zurückkehren
        DisplayMainMenu(null, shoppingCart);
    }
}
