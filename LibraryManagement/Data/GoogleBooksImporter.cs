using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Service;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class GoogleBooksImporter
    {
        private readonly LibraryManagermentContext _context;
        private readonly GoogleBooksService _googleBooksService;

        // 20 chủ đề để tìm kiếm
        private static readonly string[] Subjects = new[]
        {
            "fiction", "science", "history", "technology", "biography",
            "philosophy", "psychology", "economics", "art", "music",
            "cooking", "travel", "education", "health", "sports",
            "poetry", "drama", "comics", "religion", "politics"
        };

        public GoogleBooksImporter(LibraryManagermentContext context, HttpClient httpClient)
        {
            _context = context;
            _googleBooksService = new GoogleBooksService(httpClient);
        }

        public async Task ImportBooksAsync(int totalBooks = 200)
        {
            int booksPerSubject = totalBooks / Subjects.Length; // 10 books per subject
            Console.WriteLine($"📚 Bắt đầu import {totalBooks} sách từ {Subjects.Length} chủ đề...");

            var allBooks = await _googleBooksService.SearchBySubjectsAsync(Subjects, booksPerSubject);
            Console.WriteLine($"📖 Đã fetch được {allBooks.Count} sách từ Google Books API");

            int bookIndex = 2; // Start from book002
            int theBookIndex = 2; // Start from tb00002
            var processedTitles = new HashSet<string>();

            foreach (var book in allBooks)
            {
                if (book.VolumeInfo?.Title == null) continue;
                if (processedTitles.Contains(book.VolumeInfo.Title)) continue;
                processedTitles.Add(book.VolumeInfo.Title);

                try
                {
                    // 1. Create/Get TypeBook
                    var category = book.VolumeInfo.Categories?.FirstOrDefault() ?? "General";
                    var typeBook = await GetOrCreateTypeBookAsync(category);

                    // 2. Create/Get Author
                    var authorName = book.VolumeInfo.Authors?.FirstOrDefault() ?? "Unknown Author";
                    var author = await GetOrCreateAuthorAsync(authorName, typeBook.IdTypeBook);

                    // 3. Create HeaderBook
                    var headerBook = new HeaderBook
                    {
                        IdHeaderBook = Guid.NewGuid(),
                        IdTypeBook = typeBook.IdTypeBook,
                        NameHeaderBook = TruncateString(book.VolumeInfo.Title, 50),
                        DescribeBook = TruncateString(book.VolumeInfo.Description ?? "Không có mô tả", 500)
                    };
                    await _context.HeaderBooks.AddAsync(headerBook);

                    // 4. Create BookWriting (Author-Book relationship)
                    var bookWriting = new BookWriting
                    {
                        IdHeaderBook = headerBook.IdHeaderBook,
                        IdAuthor = author.IdAuthor
                    };
                    await _context.BookWritings.AddAsync(bookWriting);

                    // 5. Create Book
                    var newBook = new Book
                    {
                        IdBook = $"book{bookIndex:D3}",
                        IdHeaderBook = headerBook.IdHeaderBook,
                        Publisher = TruncateString(book.VolumeInfo.Publisher ?? "Unknown Publisher", 100),
                        ReprintYear = ParseYear(book.VolumeInfo.PublishedDate),
                        ValueOfBook = 50000 + new Random().Next(450000)
                    };
                    await _context.Books.AddAsync(newBook);

                    // 6. Create TheBooks (2 copies each)
                    for (int i = 0; i < 2; i++)
                    {
                        var theBook = new TheBook
                        {
                            IdTheBook = $"tb{theBookIndex:D5}",
                            IdBook = newBook.IdBook,
                            Status = i == 0 ? "Có sẵn" : "Đã mượn"
                        };
                        await _context.TheBooks.AddAsync(theBook);
                        theBookIndex++;
                    }

                    // 7. Create Image if available
                    if (book.VolumeInfo.ImageLinks?.Thumbnail != null)
                    {
                        var image = new Image
                        {
                            IdImg = Guid.NewGuid(),
                            IdBook = newBook.IdBook,
                            Url = book.VolumeInfo.ImageLinks.Thumbnail.Replace("http://", "https://")
                        };
                        await _context.Images.AddAsync(image);
                    }

                    bookIndex++;
                    
                    if (bookIndex % 20 == 0)
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"✅ Đã import {bookIndex - 2} sách...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error importing book '{book.VolumeInfo.Title}': {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"🎉 Hoàn thành! Đã import {bookIndex - 2} sách, {theBookIndex - 2} quyển.");
        }

        private async Task<TypeBook> GetOrCreateTypeBookAsync(string categoryName)
        {
            var truncatedName = TruncateString(categoryName, 20);
            var existing = await _context.TypeBooks.FirstOrDefaultAsync(t => t.NameTypeBook == truncatedName);
            
            if (existing != null) return existing;

            var newTypeBook = new TypeBook
            {
                IdTypeBook = Guid.NewGuid(),
                NameTypeBook = truncatedName
            };
            await _context.TypeBooks.AddAsync(newTypeBook);
            await _context.SaveChangesAsync();
            return newTypeBook;
        }

        private async Task<Author> GetOrCreateAuthorAsync(string authorName, Guid typeBookId)
        {
            var truncatedName = TruncateString(authorName, 40);
            var existing = await _context.Authors.FirstOrDefaultAsync(a => a.NameAuthor == truncatedName);
            
            if (existing != null) return existing;

            var newAuthor = new Author
            {
                IdAuthor = Guid.NewGuid(),
                IdTypeBook = typeBookId,
                NameAuthor = truncatedName,
                Nationality = "Unknown",
                Biography = $"Author of various books"
            };
            await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();
            return newAuthor;
        }

        private static string TruncateString(string? input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Length <= maxLength ? input : input.Substring(0, maxLength - 3) + "...";
        }

        private static int ParseYear(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString)) return 2020;
            if (int.TryParse(dateString.Split('-')[0], out int year))
            {
                return year > 1900 && year <= 2030 ? year : 2020;
            }
            return 2020;
        }
    }
}
