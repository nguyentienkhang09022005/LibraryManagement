using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public static class DbSeeder
    {
        // Password hash cho "Password123!" 
        private const string DefaultPasswordHash = "$2a$11$S9.UXD4OewZvvYI3tYXCWeLZ0/3WYwdBfnt/OJVS3C7maQwO7OrA2";
        
        public static async Task SeedAsync(LibraryManagermentContext context)
        {
            await SeedTypeBooksAsync(context);
            await SeedAuthorsAsync(context);
            await SeedHeaderBooksAsync(context);
            await DbSeederPart2.SeedBooksAsync(context);
            await DbSeederPart2.SeedTheBooksAsync(context);
            await DbSeederPart2.SeedBookWritingsAsync(context);
            await DbSeederPart2.SeedReadersAsync(context);
            await DbSeederPart2.SeedImagesAsync(context);
            await DbSeederPart2.SeedLoanSlipsAsync(context);
            await DbSeederPart2.SeedEvaluatesAsync(context);
            await DbSeederPart2.SeedFavoriteBooksAsync(context);
        }

        /// <summary>
        /// Xóa TẤT CẢ dữ liệu được seed (GIỮ NGUYÊN: roles, permissions, role_permission, reader cũ, parameters, typereader)
        /// </summary>
        public static async Task ClearSeededDataAsync(LibraryManagermentContext context)
        {
            // Xóa theo thứ tự để tôn trọng FK constraints
            // 1. Xóa các bảng phụ thuộc trước
            context.FavoriteBooks.RemoveRange(context.FavoriteBooks);
            context.Evaluates.RemoveRange(context.Evaluates);
            context.LoanSlipBooks.RemoveRange(context.LoanSlipBooks);
            
            // 2. Xóa images (trừ images của readers cũ rd00001-rd00006)
            var imagesToDelete = await context.Images
                .Where(i => i.IdReader == null || !new[] { "rd00001", "rd00002", "rd00003", "rd00004", "rd00005", "rd00006" }.Contains(i.IdReader))
                .ToListAsync();
            context.Images.RemoveRange(imagesToDelete);
            
            // 3. Xóa readers mới (rd00007+)
            var readersToDelete = await context.Readers
                .Where(r => !new[] { "rd00001", "rd00002", "rd00003", "rd00004", "rd00005", "rd00006" }.Contains(r.IdReader))
                .ToListAsync();
            context.Readers.RemoveRange(readersToDelete);
            
            // 4. Xóa TheBooks (trừ tb00001)
            var theBooksToDelete = await context.TheBooks
                .Where(t => t.IdTheBook != "tb00001")
                .ToListAsync();
            context.TheBooks.RemoveRange(theBooksToDelete);
            
            // 5. Xóa BookWritings (trừ cái đầu tiên)
            var existingBookWriting = await context.BookWritings.FirstOrDefaultAsync();
            if (existingBookWriting != null)
            {
                var bookWritingsToDelete = await context.BookWritings
                    .Where(bw => bw.IdHeaderBook != existingBookWriting.IdHeaderBook || bw.IdAuthor != existingBookWriting.IdAuthor)
                    .ToListAsync();
                context.BookWritings.RemoveRange(bookWritingsToDelete);
            }
            
            // 6. Xóa Books (trừ book001)
            var booksToDelete = await context.Books
                .Where(b => b.IdBook != "book001")
                .ToListAsync();
            context.Books.RemoveRange(booksToDelete);
            
            // 7. Xóa HeaderBooks mới (giữ cái có tên "Test")
            var headerBooksToDelete = await context.HeaderBooks
                .Where(h => h.NameHeaderBook != "Test")
                .ToListAsync();
            context.HeaderBooks.RemoveRange(headerBooksToDelete);
            
            // 8. Xóa Authors mới (giữ 3 authors cũ: Khang12, Khoa, Khang)
            var authorsToDelete = await context.Authors
                .Where(a => a.NameAuthor != "Khang12" && a.NameAuthor != "Khoa" && a.NameAuthor != "Khang")
                .ToListAsync();
            context.Authors.RemoveRange(authorsToDelete);
            
            // 9. Xóa TypeBooks mới (giữ Cổ Điển, Cổ tích)
            var typeBooksToDelete = await context.TypeBooks
                .Where(t => t.NameTypeBook != "Cổ Điển" && t.NameTypeBook != "Cổ tích")
                .ToListAsync();
            context.TypeBooks.RemoveRange(typeBooksToDelete);
            
            await context.SaveChangesAsync();
            Console.WriteLine("🗑️ Đã xóa tất cả dữ liệu được seed!");
        }

        private static async Task SeedTypeBooksAsync(LibraryManagermentContext context)
        {
            if (await context.TypeBooks.CountAsync() >= 10) return;

            var typeBooks = new List<TypeBook>
            {
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Văn học Việt Nam" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Tiểu thuyết" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Khoa học viễn tưởng" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Lịch sử" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Kinh tế" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Công nghệ" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Tâm lý học" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Triết học" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Thiếu nhi" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Truyện tranh" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Kỹ năng sống" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Y học" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Nghệ thuật" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Thể thao" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Du lịch" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Nấu ăn" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Tôn giáo" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Chính trị" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Giáo dục" },
                new() { IdTypeBook = Guid.NewGuid(), NameTypeBook = "Khoa học" },
            };

            await context.TypeBooks.AddRangeAsync(typeBooks);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAuthorsAsync(LibraryManagermentContext context)
        {
            if (await context.Authors.CountAsync() >= 20) return;

            var typeBooks = await context.TypeBooks.ToListAsync();
            var getType = (string name) => typeBooks.FirstOrDefault(t => t.NameTypeBook.Contains(name))?.IdTypeBook ?? typeBooks[0].IdTypeBook;

            var authors = new List<Author>
            {
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Nguyễn Nhật Ánh", Nationality = "Việt Nam", Biography = "Nhà văn nổi tiếng với các tác phẩm về tuổi thơ như Mắt Biếc, Tôi Thấy Hoa Vàng Trên Cỏ Xanh" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Tô Hoài", Nationality = "Việt Nam", Biography = "Tác giả Dế Mèn Phiêu Lưu Ký, nhà văn lớn của văn học thiếu nhi Việt Nam" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Nam Cao", Nationality = "Việt Nam", Biography = "Nhà văn hiện thực phê phán với Chí Phèo, Lão Hạc" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Nguyễn Du", Nationality = "Việt Nam", Biography = "Đại thi hào dân tộc, tác giả Truyện Kiều" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Nguyễn Ngọc Tư", Nationality = "Việt Nam", Biography = "Nhà văn đương đại với Cánh Đồng Bất Tận" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Khoa học"), NameAuthor = "Stephen Hawking", Nationality = "Anh", Biography = "Nhà vật lý lý thuyết, tác giả Lược Sử Thời Gian" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Lịch sử"), NameAuthor = "Yuval Noah Harari", Nationality = "Israel", Biography = "Sử gia, tác giả Sapiens và Homo Deus" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameAuthor = "Robert Kiyosaki", Nationality = "Mỹ", Biography = "Tác giả Rich Dad Poor Dad" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameAuthor = "Dale Carnegie", Nationality = "Mỹ", Biography = "Tác giả Đắc Nhân Tâm" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "J.K. Rowling", Nationality = "Anh", Biography = "Tác giả Harry Potter" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "Paulo Coelho", Nationality = "Brazil", Biography = "Tác giả Nhà Giả Kim" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "Haruki Murakami", Nationality = "Nhật Bản", Biography = "Nhà văn đương đại Nhật Bản" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "George Orwell", Nationality = "Anh", Biography = "Tác giả 1984 và Trại Súc Vật" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameAuthor = "Robert C. Martin", Nationality = "Mỹ", Biography = "Uncle Bob, tác giả Clean Code" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameAuthor = "Martin Kleppmann", Nationality = "Đức", Biography = "Tác giả Designing Data-Intensive Applications" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameAuthor = "Napoleon Hill", Nationality = "Mỹ", Biography = "Tác giả Think and Grow Rich" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameAuthor = "James Clear", Nationality = "Mỹ", Biography = "Tác giả Atomic Habits" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameAuthor = "Morgan Housel", Nationality = "Mỹ", Biography = "Tác giả The Psychology of Money" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameAuthor = "Fujiko F. Fujio", Nationality = "Nhật Bản", Biography = "Tác giả Doraemon" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameAuthor = "Gosho Aoyama", Nationality = "Nhật Bản", Biography = "Tác giả Thám Tử Lừng Danh Conan" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameAuthor = "Vũ Trọng Phụng", Nationality = "Việt Nam", Biography = "Tác giả Số Đỏ, nhà văn trào phúng" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameAuthor = "Eiichiro Oda", Nationality = "Nhật Bản", Biography = "Tác giả One Piece" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "Dan Brown", Nationality = "Mỹ", Biography = "Tác giả Mật Mã Da Vinci" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Thiếu nhi"), NameAuthor = "Nguyễn Quang Sáng", Nationality = "Việt Nam", Biography = "Tác giả Chiếc Lược Ngà" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Khoa học"), NameAuthor = "Carl Sagan", Nationality = "Mỹ", Biography = "Nhà thiên văn học, tác giả Cosmos" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameAuthor = "Viktor Frankl", Nationality = "Áo", Biography = "Tác giả Man's Search for Meaning" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameAuthor = "Gabriel García Márquez", Nationality = "Colombia", Biography = "Nobel Văn học, tác giả Trăm Năm Cô Đơn" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Triết học"), NameAuthor = "Friedrich Nietzsche", Nationality = "Đức", Biography = "Triết gia lừng danh" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Kỹ năng"), NameAuthor = "Stephen Covey", Nationality = "Mỹ", Biography = "Tác giả 7 Thói Quen Hiệu Quả" },
                new() { IdAuthor = Guid.NewGuid(), IdTypeBook = getType("Y học"), NameAuthor = "Atul Gawande", Nationality = "Mỹ", Biography = "Bác sĩ phẫu thuật, tác giả The Checklist Manifesto" },
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }

        private static async Task SeedHeaderBooksAsync(LibraryManagermentContext context)
        {
            if (await context.HeaderBooks.CountAsync() >= 30) return;

            var typeBooks = await context.TypeBooks.ToListAsync();
            var getType = (string name) => typeBooks.FirstOrDefault(t => t.NameTypeBook.Contains(name))?.IdTypeBook ?? typeBooks[0].IdTypeBook;

            var headerBooks = GetHeaderBookData(getType);
            await context.HeaderBooks.AddRangeAsync(headerBooks);
            await context.SaveChangesAsync();
        }

        private static List<HeaderBook> GetHeaderBookData(Func<string, Guid> getType)
        {
            return new List<HeaderBook>
            {
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Mắt Biếc", DescribeBook = "Câu chuyện tình yêu đầy day dứt của Ngạn dành cho Hà Lan, từ thuở ấu thơ đến khi trưởng thành. Một trong những tác phẩm xuất sắc nhất của Nguyễn Nhật Ánh." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Cho Tôi Xin Một Vé Đi Tuổi Thơ", DescribeBook = "Hành trình trở về tuổi thơ với những kỷ niệm đẹp đẽ, trong sáng và đầy tiếng cười của một thời đã qua." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Thiếu nhi"), NameHeaderBook = "Dế Mèn Phiêu Lưu Ký", DescribeBook = "Cuộc phiêu lưu của chú Dế Mèn qua bao vùng đất, gặp gỡ nhiều bạn bè và học được nhiều bài học cuộc sống." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Chí Phèo", DescribeBook = "Bi kịch của người nông dân bị tha hóa trong xã hội phong kiến. Tác phẩm tiêu biểu của văn học hiện thực phê phán Việt Nam." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Truyện Kiều", DescribeBook = "Kiệt tác văn học cổ điển Việt Nam, kể về cuộc đời đầy sóng gió của Thúy Kiều với 3254 câu thơ lục bát." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Khoa học"), NameHeaderBook = "Lược Sử Thời Gian", DescribeBook = "Stephen Hawking giải thích về vũ trụ, Big Bang, lỗ đen và bản chất của thời gian một cách dễ hiểu cho mọi người." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Lịch sử"), NameHeaderBook = "Sapiens: Lược Sử Loài Người", DescribeBook = "Hành trình 70.000 năm của loài người từ động vật thường thành bá chủ Trái Đất, qua các cuộc cách mạng nhận thức, nông nghiệp và khoa học." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameHeaderBook = "Cha Giàu Cha Nghèo", DescribeBook = "Bài học tài chính từ hai người cha với quan điểm khác nhau về tiền bạc, giúp thay đổi tư duy về đầu tư và làm giàu." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameHeaderBook = "Đắc Nhân Tâm", DescribeBook = "Cuốn sách kinh điển về nghệ thuật giao tiếp và thu phục lòng người, đã thay đổi cuộc đời hàng triệu độc giả." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Harry Potter và Hòn Đá Phù Thủy", DescribeBook = "Cuộc phiêu lưu bắt đầu của Harry Potter tại trường Hogwarts, khám phá thế giới phù thủy kỳ diệu." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Harry Potter và Phòng Chứa Bí Mật", DescribeBook = "Harry quay lại Hogwarts và đối mặt với bí ẩn về Phòng Chứa Bí Mật, nơi ẩn chứa quái vật đáng sợ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Nhà Giả Kim", DescribeBook = "Hành trình của Santiago đi tìm kho báu và khám phá ra ý nghĩa thực sự của cuộc sống và ước mơ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Rừng Na Uy", DescribeBook = "Câu chuyện tình yêu, mất mát và trưởng thành của Toru Watanabe trong bối cảnh Nhật Bản những năm 1960." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "1984", DescribeBook = "Viễn cảnh đen tối về xã hội toàn trị, nơi Đảng kiểm soát mọi khía cạnh của cuộc sống và tư tưởng con người." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameHeaderBook = "Clean Code", DescribeBook = "Hướng dẫn viết code sạch, dễ đọc và dễ bảo trì. Cuốn sách gối đầu giường của mọi lập trình viên." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameHeaderBook = "Designing Data-Intensive Applications", DescribeBook = "Phân tích sâu về thiết kế hệ thống xử lý dữ liệu lớn, từ database đến distributed systems." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh", DescribeBook = "Những kỷ niệm tuổi thơ ở làng quê, tình anh em và những bài học về sự chia sẻ, tha thứ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Cánh Đồng Bất Tận", DescribeBook = "Câu chuyện về cuộc sống du mục của cha con trên cánh đồng miền Tây, đầy bi thương và nhân văn." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Lịch sử"), NameHeaderBook = "Homo Deus", DescribeBook = "Tương lai của loài người khi công nghệ sinh học và trí tuệ nhân tạo thay đổi bản chất con người." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameHeaderBook = "Think and Grow Rich", DescribeBook = "13 nguyên tắc thành công được đúc kết từ cuộc đời của 500 người giàu nhất nước Mỹ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameHeaderBook = "Atomic Habits", DescribeBook = "Phương pháp xây dựng thói quen tốt và loại bỏ thói quen xấu thông qua những thay đổi nhỏ hàng ngày." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameHeaderBook = "The Psychology of Money", DescribeBook = "Tâm lý học về tiền bạc - hiểu cách con người nghĩ về tiền và đưa ra quyết định tài chính." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameHeaderBook = "Doraemon Tập 1", DescribeBook = "Chú mèo máy đến từ tương lai với túi thần kỳ, giúp đỡ cậu bé Nobita." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameHeaderBook = "Thám Tử Lừng Danh Conan Tập 1", DescribeBook = "Shinichi bị thu nhỏ thành Conan, bắt đầu hành trình phá án và tìm cách trở lại hình dáng ban đầu." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Số Đỏ", DescribeBook = "Tiểu thuyết trào phúng về xã hội Việt Nam thời Pháp thuộc qua nhân vật Xuân Tóc Đỏ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameHeaderBook = "One Piece Tập 1", DescribeBook = "Hành trình của Luffy và băng Mũ Rơm tìm kiếm kho báu One Piece và trở thành Vua Hải Tặc." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Mật Mã Da Vinci", DescribeBook = "Cuộc điều tra về vụ giết người tại Louvre dẫn đến bí mật lớn nhất trong lịch sử tôn giáo." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Thiếu nhi"), NameHeaderBook = "Chiếc Lược Ngà", DescribeBook = "Tình cha con sâu nặng trong hoàn cảnh chiến tranh, chiếc lược ngà là biểu tượng của tình yêu thương." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Khoa học"), NameHeaderBook = "Cosmos", DescribeBook = "Hành trình khám phá vũ trụ qua góc nhìn của nhà thiên văn học Carl Sagan." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameHeaderBook = "Đi Tìm Lẽ Sống", DescribeBook = "Viktor Frankl chia sẻ trải nghiệm trong trại tập trung và triết lý về ý nghĩa cuộc sống." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Trăm Năm Cô Đơn", DescribeBook = "Lịch sử 7 thế hệ gia đình Buendía ở Macondo, kiệt tác của văn học Mỹ Latin." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Triết học"), NameHeaderBook = "Zarathustra Đã Nói Như Thế", DescribeBook = "Tác phẩm triết học của Nietzsche về siêu nhân và ý chí quyền lực." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Kỹ năng"), NameHeaderBook = "7 Thói Quen Hiệu Quả", DescribeBook = "7 nguyên tắc để phát triển bản thân và đạt hiệu quả trong công việc lẫn cuộc sống." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Y học"), NameHeaderBook = "The Checklist Manifesto", DescribeBook = "Sức mạnh của checklist trong y khoa và các lĩnh vực phức tạp khác." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Văn học"), NameHeaderBook = "Lão Hạc", DescribeBook = "Bi kịch của người nông dân nghèo phải bán con chó Vàng yêu quý." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Biên Niên Ký Chim Vặn Dây Cót", DescribeBook = "Cuộc tìm kiếm kỳ lạ của Toru Okada về con mèo và người vợ mất tích." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Kafka Bên Bờ Biển", DescribeBook = "Hành trình song song của cậu bé 15 tuổi và ông lão nói chuyện được với mèo." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Trại Súc Vật", DescribeBook = "Ngụ ngôn chính trị về cuộc cách mạng của loài vật và sự tha hóa quyền lực." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Harry Potter và Tên Tù Nhân Azkaban", DescribeBook = "Harry khám phá bí mật về Sirius Black - tên tù nhân nguy hiểm vượt ngục." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tiểu thuyết"), NameHeaderBook = "Harry Potter và Chiếc Cốc Lửa", DescribeBook = "Giải đấu Tam Pháp Thuật và sự trở lại của Chúa tể Hắc ám Voldemort." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameHeaderBook = "The Pragmatic Programmer", DescribeBook = "Hướng dẫn thực hành để trở thành lập trình viên chuyên nghiệp." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Công nghệ"), NameHeaderBook = "Introduction to Algorithms", DescribeBook = "Sách giáo khoa kinh điển về thuật toán và cấu trúc dữ liệu." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Kinh tế"), NameHeaderBook = "The Intelligent Investor", DescribeBook = "Cuốn sách đầu tư giá trị của Benjamin Graham, thầy của Warren Buffett." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Tâm lý"), NameHeaderBook = "Thinking, Fast and Slow", DescribeBook = "Hai hệ thống tư duy của con người và những sai lầm nhận thức phổ biến." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameHeaderBook = "Dragon Ball Tập 1", DescribeBook = "Hành trình tìm ngọc rồng của Goku từ khi còn nhỏ." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Truyện tranh"), NameHeaderBook = "Naruto Tập 1", DescribeBook = "Câu chuyện về ninja Naruto và ước mơ trở thành Hokage." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Du lịch"), NameHeaderBook = "Tôi Đi Tìm Tôi", DescribeBook = "Hành trình du lịch bụi khám phá bản thân qua các vùng đất." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Nấu ăn"), NameHeaderBook = "Món Việt Truyền Thống", DescribeBook = "Công thức nấu các món ăn truyền thống Việt Nam." },
                new() { IdHeaderBook = Guid.NewGuid(), IdTypeBook = getType("Giáo dục"), NameHeaderBook = "Montessori Từ Đầu", DescribeBook = "Phương pháp giáo dục Montessori cho trẻ nhỏ." },
            };
        }
    }
}
