using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public static class DbSeederPart2
    {
        private const string DefaultPasswordHash = "$2a$11$S9.UXD4OewZvvYI3tYXCWeLZ0/3WYwdBfnt/OJVS3C7maQwO7OrA2";
        private static readonly string[] Publishers = { "NXB Trẻ", "NXB Kim Đồng", "Alpha Books", "First News", "NXB Văn Học", "NXB Tổng Hợp", "NXB Giáo Dục", "Nhã Nam" };

        public static async Task SeedBooksAsync(LibraryManagermentContext context)
        {
            if (await context.Books.CountAsync() >= 30) return;

            var headerBooks = await context.HeaderBooks.ToListAsync();
            var books = new List<Book>();
            var random = new Random(42);
            int idx = 2; // Start from book002 (book001 exists)

            foreach (var header in headerBooks.Take(50))
            {
                books.Add(new Book
                {
                    IdBook = $"book{idx:D3}",
                    IdHeaderBook = header.IdHeaderBook,
                    Publisher = Publishers[random.Next(Publishers.Length)],
                    ReprintYear = 2020 + random.Next(6),
                    ValueOfBook = 50000 + random.Next(450000)
                });
                idx++;
            }

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }

        public static async Task SeedTheBooksAsync(LibraryManagermentContext context)
        {
            if (await context.TheBooks.CountAsync() >= 100) return;

            var books = await context.Books.ToListAsync();
            var theBooks = new List<TheBook>();
            int idx = 2; // tb00002

            foreach (var book in books)
            {
                for (int i = 0; i < 4; i++) // 4 copies each
                {
                    theBooks.Add(new TheBook
                    {
                        IdTheBook = $"tb{idx:D5}",
                        IdBook = book.IdBook,
                        Status = i < 3 ? "Có sẵn" : "Đã mượn"
                    });
                    idx++;
                }
            }

            await context.TheBooks.AddRangeAsync(theBooks);
            await context.SaveChangesAsync();
        }

        public static async Task SeedBookWritingsAsync(LibraryManagermentContext context)
        {
            if (await context.BookWritings.CountAsync() >= 20) return;

            var authors = await context.Authors.ToListAsync();
            var headerBooks = await context.HeaderBooks.ToListAsync();
            var bookWritings = new List<BookWriting>();

            for (int i = 0; i < Math.Min(authors.Count, headerBooks.Count); i++)
            {
                bookWritings.Add(new BookWriting
                {
                    IdHeaderBook = headerBooks[i].IdHeaderBook,
                    IdAuthor = authors[i % authors.Count].IdAuthor
                });
            }

            await context.BookWritings.AddRangeAsync(bookWritings);
            await context.SaveChangesAsync();
        }

        // New Readers - Password: Password123!
        public static async Task SeedReadersAsync(LibraryManagermentContext context)
        {
            if (await context.Readers.CountAsync() >= 15) return;

            var typeReader = await context.TypeReaders.FirstOrDefaultAsync(t => t.NameTypeReader == "READER");
            if (typeReader == null) return;

            var readers = new List<Reader>
            {
                new() { IdReader = "rd00007", IdTypeReader = typeReader.IdTypeReader, NameReader = "Nguyễn Văn An", Sex = "Nam", Email = "nguyenvanan@gmail.com", Phone = "0901234567", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00008", IdTypeReader = typeReader.IdTypeReader, NameReader = "Trần Thị Bình", Sex = "Nữ", Email = "tranthibibh@gmail.com", Phone = "0912345678", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00009", IdTypeReader = typeReader.IdTypeReader, NameReader = "Lê Hoàng Cường", Sex = "Nam", Email = "lehoangcuong@gmail.com", Phone = "0923456789", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00010", IdTypeReader = typeReader.IdTypeReader, NameReader = "Phạm Minh Dũng", Sex = "Nam", Email = "phamminhdung@gmail.com", Phone = "0934567890", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00011", IdTypeReader = typeReader.IdTypeReader, NameReader = "Hoàng Thị Em", Sex = "Nữ", Email = "hoangthiem@gmail.com", Phone = "0945678901", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00012", IdTypeReader = typeReader.IdTypeReader, NameReader = "Vũ Đức Phong", Sex = "Nam", Email = "vuducphong@gmail.com", Phone = "0956789012", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00013", IdTypeReader = typeReader.IdTypeReader, NameReader = "Đỗ Thị Giang", Sex = "Nữ", Email = "dothigiang@gmail.com", Phone = "0967890123", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00014", IdTypeReader = typeReader.IdTypeReader, NameReader = "Bùi Văn Hải", Sex = "Nam", Email = "buivanhai@gmail.com", Phone = "0978901234", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00015", IdTypeReader = typeReader.IdTypeReader, NameReader = "Ngô Thị Ivy", Sex = "Nữ", Email = "ngothiivy@gmail.com", Phone = "0989012345", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
                new() { IdReader = "rd00016", IdTypeReader = typeReader.IdTypeReader, NameReader = "Trịnh Quốc Khánh", Sex = "Nam", Email = "trinhquockhanh@gmail.com", Phone = "0990123456", CreateDate = DateTime.UtcNow, ReaderPassword = DefaultPasswordHash, TotalDebt = 0, RoleName = "Reader" },
            };

            await context.Readers.AddRangeAsync(readers);
            await context.SaveChangesAsync();
        }

        public static async Task SeedImagesAsync(LibraryManagermentContext context)
        {
            if (await context.Images.CountAsync() >= 20) return;

            var books = await context.Books.Take(20).ToListAsync();
            var authors = await context.Authors.Take(10).ToListAsync();
            var images = new List<Image>();

            foreach (var book in books)
            {
                images.Add(new Image
                {
                    IdImg = Guid.NewGuid(),
                    IdBook = book.IdBook,
                    Url = $"https://picsum.photos/seed/{book.IdBook}/300/400"
                });
            }

            foreach (var author in authors)
            {
                images.Add(new Image
                {
                    IdImg = Guid.NewGuid(),
                    IdAuthor = author.IdAuthor,
                    Url = $"https://picsum.photos/seed/{author.IdAuthor}/200/200"
                });
            }

            await context.Images.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        public static async Task SeedLoanSlipsAsync(LibraryManagermentContext context)
        {
            if (await context.LoanSlipBooks.AnyAsync()) return;

            var readers = await context.Readers.Where(r => r.RoleName == "Reader").Take(10).ToListAsync();
            var theBooks = await context.TheBooks.Where(t => t.Status == "Đã mượn").Take(20).ToListAsync();
            var loanSlips = new List<LoanSlipBook>();

            for (int i = 0; i < Math.Min(readers.Count, theBooks.Count); i++)
            {
                loanSlips.Add(new LoanSlipBook
                {
                    IdLoanSlipBook = Guid.NewGuid(),
                    IdTheBook = theBooks[i].IdTheBook,
                    IdReader = readers[i % readers.Count].IdReader,
                    BorrowDate = DateTime.UtcNow.AddDays(-10 - i),
                    LoanPeriod = 14,
                    FineAmount = 0,
                    IsReturned = false
                });
            }

            await context.LoanSlipBooks.AddRangeAsync(loanSlips);
            await context.SaveChangesAsync();
        }

        public static async Task SeedEvaluatesAsync(LibraryManagermentContext context)
        {
            if (await context.Evaluates.AnyAsync()) return;

            var readers = await context.Readers.Take(10).ToListAsync();
            var books = await context.Books.Take(15).ToListAsync();
            var evaluates = new List<Evaluate>();
            var comments = new[] { "Sách rất hay!", "Nội dung hấp dẫn", "Đáng đọc", "Tuyệt vời", "Rất bổ ích" };
            var random = new Random(42);

            for (int i = 0; i < Math.Min(readers.Count * 2, books.Count); i++)
            {
                evaluates.Add(new Evaluate
                {
                    IdEvaluate = Guid.NewGuid(),
                    IdReader = readers[i % readers.Count].IdReader,
                    IdBook = books[i].IdBook,
                    EvaComment = comments[random.Next(comments.Length)],
                    EvaStar = 3 + random.Next(3),
                    CreateDate = DateTime.UtcNow.AddDays(-random.Next(30))
                });
            }

            await context.Evaluates.AddRangeAsync(evaluates);
            await context.SaveChangesAsync();
        }

        public static async Task SeedFavoriteBooksAsync(LibraryManagermentContext context)
        {
            if (await context.FavoriteBooks.AnyAsync()) return;

            var readers = await context.Readers.Take(8).ToListAsync();
            var books = await context.Books.Take(10).ToListAsync();
            var favorites = new List<FavoriteBook>();

            for (int i = 0; i < readers.Count; i++)
            {
                favorites.Add(new FavoriteBook
                {
                    IdReader = readers[i].IdReader,
                    IdBook = books[i % books.Count].IdBook,
                    createDay = DateTime.UtcNow.AddDays(-i)
                });
            }

            await context.FavoriteBooks.AddRangeAsync(favorites);
            await context.SaveChangesAsync();
        }
    }
}
