using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace LibraryManagement.Repository
{
    public class BookService : IBookService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IBookReceiptService _bookReceiptRepository;
        private readonly IUpLoadImageFileService _upLoadImageFileRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IAuthenService _authen; 

        public BookService(LibraryManagermentContext context, 
                              IMapper mapper, 
                              IBookReceiptService bookReceiptRepository,
                              IUpLoadImageFileService upLoadImageFileRepository,
                              IParameterService parameterRepository, 
                              IAuthenService authen
            )
        {
            _context = context;
            _bookReceiptRepository = bookReceiptRepository;
            _upLoadImageFileRepository = upLoadImageFileRepository;
            _parameterRepository = parameterRepository;
            _authen = authen; 
        }

        // Hàm thêm mới đầu sách
        public async Task<ApiResponse<HeaderBookResponse>> addBookAsync(HeaderBookCreationRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                // Quy định khoảng cách năm xuất bản
                int publishBookGap = DateTime.Now.Year - request.bookCreateRequest.ReprintYear;
                int publishGap = await _parameterRepository.getValueAsync("PublishGap");
                if (publishBookGap > publishGap)
                {
                    return ApiResponse<HeaderBookResponse>.FailResponse($"Khoảng cách năm xuất bản phải nhỏ hơn {publishGap}", 400);
                }

                // Tạo đầu sách
                string imageUrl = null!;

                var typeBook = await _context.TypeBooks.AsNoTracking()
                    .FirstOrDefaultAsync(typebook => typebook.IdTypeBook == request.IdTypeBook);

                var headerBook = await _context.HeaderBooks
                    .FirstOrDefaultAsync(hb => hb.NameHeaderBook == request.NameHeaderBook);

                if (typeBook == null)
                {
                    return ApiResponse<HeaderBookResponse>.FailResponse("Không tìm thấy loại sách phù hợp", 404);
                }

                if (headerBook == null)
                {
                    headerBook = new HeaderBook
                    {
                        IdTypeBook = request.IdTypeBook,
                        NameHeaderBook = request.NameHeaderBook,
                        DescribeBook = request.DescribeBook,
                    };
                    _context.HeaderBooks.Add(headerBook);
                    await _context.SaveChangesAsync();

                    if (request.Authors != null && request.Authors.Any()) // Duyệt qua danh sách tác giả
                    {
                        foreach (var authorId in request.Authors)
                        {
                            var bookWriting = new BookWriting
                            {
                                IdHeaderBook = headerBook.IdHeaderBook,
                                IdAuthor = authorId
                            };
                            _context.BookWritings.Add(bookWriting); // Nạp dữ liệu vào bảng sáng tác
                        }
                    }
                }
                else
                {
                    headerBook.DescribeBook = request.DescribeBook; // Luôn cập nhật Describe của Book
                    _context.HeaderBooks.Update(headerBook);
                    await _context.SaveChangesAsync();
                }

                // Tạo sách
                // Chuỗi url ảnh từ cloudinary
                if (request.BookImage != null)
                {
                    imageUrl = await _upLoadImageFileRepository.UploadImageAsync(request.BookImage);
                }
                var book = new Book
                {
                    IdBook = await _bookReceiptRepository.generateNextIdBookAsync(),
                    IdHeaderBook = headerBook.IdHeaderBook,
                    Publisher = request.bookCreateRequest.Publisher,
                    ReprintYear = request.bookCreateRequest.ReprintYear,
                    ValueOfBook = request.bookCreateRequest.ValueOfBook
                };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                // Lưu ảnh sách vào bảng image nếu có
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var image = new Image
                    {
                        IdBook = book.IdBook,
                        Url = imageUrl,
                    };
                    _context.Images.Add(image);
                    await _context.SaveChangesAsync();
                }

                // Tạo cuốn sách
                var theBook = new TheBook
                {
                    IdTheBook = await _bookReceiptRepository.generateNextIdTheBookAsync(),
                    IdBook = book.IdBook,
                    Status = "Có sẵn"
                };
                _context.TheBooks.Add(theBook);
                await _context.SaveChangesAsync();

                // Lấy thông tin tác giả cuốn của sách
                var authorOfBook = await _context.BookWritings
                    .Where(bw => bw.IdHeaderBook == headerBook.IdHeaderBook)
                    .Include(bw => bw.Author)
                    .ToListAsync();

                var authors = authorOfBook.Select(a => new AuthorOfBookResponse
                {
                    IdAuthor = a.Author.IdAuthor,
                    NameAuthor = a.Author.NameAuthor
                }).ToList();

                // Lấy hình ảnh cuốn sách
                var imageBook = await _context.Images
                    .Where(img => img.IdBook == book.IdBook)
                    .Select(img => img.Url)
                    .FirstOrDefaultAsync();

                // Ánh xạ response
                var response = new HeaderBookResponse
                {
                    TypeBook = new TypeBookResponse
                    {
                        IdTypeBook = typeBook.IdTypeBook,
                        NameTypeBook = typeBook.NameTypeBook
                    },
                    NameHeaderBook = headerBook.NameHeaderBook,
                    DescribeBook = headerBook.DescribeBook,
                    Authors = authors,

                    bookResponse = new BookResponse
                    {
                        IdBook = book.IdBook,
                        Publisher = book.Publisher,
                        ReprintYear = book.ReprintYear,
                        ValueOfBook = book.ValueOfBook,
                        UrlImage = imageBook,
                    },
                    thebookReponse = new TheBookResponse
                    {
                        IdTheBook = theBook.IdTheBook,
                        Status = theBook.Status
                    }
                };
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ApiResponse<HeaderBookResponse>.SuccessResponse("Tạo sách thành công", 201, response);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Hàm xóa sách
        public async Task<ApiResponse<string>> deleteBookAsync(string idBook)
        {
            // Tìm Book
            var book = await _context.Books.FirstOrDefaultAsync(b => b.IdBook == idBook.ToString());
            if (book == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy sách", 404);
            }

            // Tìm các cuốn sách thuộc Book
            var theBooks = await _context.TheBooks
                .Where(tb => tb.IdBook == idBook.ToString())
                .ToListAsync();

            _context.TheBooks.RemoveRange(theBooks);
            _context.Books.Remove(book);

            // Kiểm tra có còn bản sách nào cho đầu sách này không
            int remainingBooks = await _context.Books
                .CountAsync(b => b.IdHeaderBook == book.IdHeaderBook && b.IdBook != idBook.ToString());

            if (remainingBooks == 0)
            {
                var headerBook = await _context.HeaderBooks // Xóa đầu sách
                    .FirstOrDefaultAsync(hb => hb.IdHeaderBook == book.IdHeaderBook);

                var createBooks = await _context.BookWritings // Xóa sáng tác của tác giả
                    .Where(cb => cb.IdHeaderBook == book.IdHeaderBook)
                    .ToListAsync();

                _context.BookWritings.RemoveRange(createBooks);
                if (headerBook != null)
                {
                    _context.HeaderBooks.Remove(headerBook);
                }
            }
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Xóa sách thành công", 200, null!);
        }

        // Hàm cập nhật sách
        public async Task<ApiResponse<HeaderBookResponse>> updateBookAsync(HeaderBookUpdateRequest request, 
                                                                           string idBook, string idTheBook)
        {
            // Quy định khoảng cách năm xuất bản
            int publishBookGap = DateTime.Now.Year - request.bookUpdateRequest.ReprintYear;
            int publishGap = await _parameterRepository.getValueAsync("PublishGap");
            if (publishBookGap > publishGap)
            {
                return ApiResponse<HeaderBookResponse>.FailResponse($"Khoảng cách năm xuất bản phải nhỏ hơn {publishGap}", 400);
            }

            var checkBook = await _context.Books.FirstOrDefaultAsync(b => b.IdBook == idBook.ToString());
            if (checkBook == null)
            {
                return ApiResponse<HeaderBookResponse>.FailResponse("Không tìm thấy sách", 404);
            }

            var typeBook = await _context.TypeBooks.FirstOrDefaultAsync(typebook => typebook.IdTypeBook == request.IdTypeBook);
            if (typeBook == null)
            {
                return ApiResponse<HeaderBookResponse>.FailResponse("Không tìm thấy loại sách phù hợp", 404);
            }

            var headerBook = await _context.HeaderBooks.FirstOrDefaultAsync(hb => hb.NameHeaderBook == request.NameHeaderBook);
            // Tạo đầu sách
            if (headerBook == null)
            {
                headerBook = new HeaderBook
                {
                    IdTypeBook = request.IdTypeBook,
                    NameHeaderBook = request.NameHeaderBook,
                    DescribeBook = request.DescribeBook,
                };
                await _context.HeaderBooks.AddAsync(headerBook);
                await _context.SaveChangesAsync();
            }else
            {
                headerBook.DescribeBook = request.DescribeBook; // Luôn cập nhật Describe của Book
                _context.HeaderBooks.Update(headerBook);
                await _context.SaveChangesAsync();
            }

            // Xoá toàn bộ tác giả cũ
            var oldBookWritings = await _context.BookWritings
                .Where(bw => bw.IdHeaderBook == headerBook.IdHeaderBook)
                .ToListAsync();

            _context.BookWritings.RemoveRange(oldBookWritings);

            if (request.IdAuthors != null && request.IdAuthors.Any()) // Duyệt qua danh sách tác giả
            {
                foreach (var authorId in request.IdAuthors)
                {
                    var createBook = new BookWriting
                    {
                        IdHeaderBook = headerBook.IdHeaderBook,
                        IdAuthor = authorId
                    };
                    await _context.BookWritings.AddAsync(createBook); // Nạp dữ liệu vào bảng sáng tác
                }
            }
            await _context.SaveChangesAsync();

            // Cập nhật thông tin sách
            checkBook.Publisher = request.bookUpdateRequest.Publisher;
            checkBook.ReprintYear = request.bookUpdateRequest.ReprintYear;
            checkBook.ValueOfBook = request.bookUpdateRequest.ValueOfBook;
            checkBook.IdHeaderBook = headerBook.IdHeaderBook;

            var checkTheBook = await _context.TheBooks.FirstOrDefaultAsync(tb => tb.IdTheBook == idTheBook.ToString());
            if (checkTheBook == null)
            {
                return ApiResponse<HeaderBookResponse>.FailResponse("Không tìm thấy cuốn sách", 404);
            }
            // Cập nhật thông tin cuốn sách
            checkTheBook.Status = request.theBookUpdateRequest.Status;
            await _context.SaveChangesAsync();

            // Cập nhật hình ảnh
            if (request.BookImage != null && request.BookImage.Length > 0)
            {
                var imageUrl = await _upLoadImageFileRepository.UploadImageAsync(request.BookImage);

                var image = await _context.Images
                    .FirstOrDefaultAsync(img => img.IdBook == checkBook.IdBook);

                if (image != null) // Nếu đã có ảnh rồi thì cập nhật
                {
                    image.Url = imageUrl;
                    _context.Images.Update(image);
                }
                else // Nếu chưa có thì thêm mới
                {
                    var newImage = new Image
                    {
                        IdBook = checkBook.IdBook,
                        Url = imageUrl
                    };
                    await _context.Images.AddAsync(newImage);
                }

                await _context.SaveChangesAsync();
            }

            // Lấy thông tin tác giả cuốn của sách
            var authorOfBook = await _context.BookWritings
                .Where(bw => bw.IdHeaderBook == headerBook.IdHeaderBook)
                .Include(bw => bw.Author)
                .ToListAsync();

            var authors = authorOfBook.Select(a => new AuthorOfBookResponse
            {
                IdAuthor = a.Author.IdAuthor,
                NameAuthor = a.Author.NameAuthor
            }).ToList();

            // Lấy hình ảnh cuốn sách
            var imageBook = await _context.Images
                .Where(img => img.IdBook == checkBook.IdBook)
                .Select(img => img.Url)
                .FirstOrDefaultAsync();

            // Ánh xạ response
            var response = new HeaderBookResponse
            {
                TypeBook = new TypeBookResponse
                {
                    IdTypeBook = typeBook.IdTypeBook,
                    NameTypeBook = typeBook.NameTypeBook
                },
                NameHeaderBook = headerBook.NameHeaderBook,
                DescribeBook = headerBook.DescribeBook,
                Authors = authors,

                bookResponse = new BookResponse
                {
                    IdBook = checkBook.IdBook,
                    Publisher = checkBook.Publisher,
                    ReprintYear = checkBook.ReprintYear,
                    ValueOfBook = checkBook.ValueOfBook,
                    UrlImage = imageBook,
                },
                thebookReponse = new TheBookResponse
                {
                    IdTheBook = checkTheBook.IdTheBook,
                    Status = checkTheBook.Status
                }
            };
            return ApiResponse<HeaderBookResponse>.SuccessResponse("Cập nhật sách thành công", 201, response);
        }


     
        public async Task<List<EvaluationDetails>> getBooksEvaluation(EvaluationDetailInput dto)
        {
          
            var result = await _context.Evaluates.AsNoTracking().Where(x => x.IdBook == dto.IdBook).Take(50).Select(a => new EvaluationDetails
            {
                IdEvaluation = a.IdEvaluate,
                IdReader = a.IdReader,
                Comment = a.EvaComment,
                Rating = a.EvaStar,
                Create_Date = a.CreateDate
            }).ToListAsync();
            return result;
        }


        //
        public async Task<ApiResponse<bool>> LikeBook(EvaluationDetailInput dto)
        {
          
            var likedBook = await _context.FavoriteBooks.AsNoTracking().Where(x => x.IdReader == dto.idUser && x.IdBook == dto.IdBook).FirstOrDefaultAsync();
            if (likedBook != null)
            {
                await _context.FavoriteBooks.AsNoTracking().Where(x=>x.IdReader == dto.idUser && x.IdBook == dto.IdBook).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(false, "Bỏ like thành công", 200, data: false);
             
            }
            else
            {
                var likeBook = new FavoriteBook
                {
                    IdBook = dto.IdBook,
                    IdReader = dto.idUser,
                    createDay = DateTime.UtcNow
                };
                await _context.LikedHeaderBooks.AddAsync(likeBook);
                await _context.SaveChangesAsync();
                return new ApiResponse<bool>(false, "Like thành công", 200, data: false);
            }
        }

        public async Task<List<BooksAndComments>> getAllBooksInDetail(string reeaderId)
        {
            var result = await _context.Books
                .AsNoTracking()
                .Include(a => a.HeaderBook)
                .ThenInclude(x => x.bookWritings)
                .ThenInclude(c => c.Author)
                .Include(a => a.Evaluates)
                .Include(a=>a.images)
                .Select(x => new BooksAndComments
                {
                    idBook = x.IdBook,
                    nameBook = x.HeaderBook.NameHeaderBook,
                    describe = x.HeaderBook.DescribeBook,
                    isLiked = _context.FavoriteBooks.Any(k => k.IdReader == reeaderId && k.IdBook == x.IdBook),
                    valueOfbook = x.ValueOfBook,
                    Evaluations = _context.Evaluates
                                .Where(a => a.IdBook == x.IdBook)
                                .Select(a => new EvaluationDetails
                                {
                                    IdEvaluation = a.IdEvaluate,
                                    IdReader = a.IdReader,
                                    Comment = a.EvaComment,
                                    Rating = a.EvaStar,
                                    Create_Date = a.CreateDate
                                }).ToList(),
                    Authors = x.HeaderBook.bookWritings
                              .Select(a => new AuthorResponse
                              {
                                  IdAuthor = a.IdAuthor,
                                  NameAuthor = a.Author.NameAuthor,
                                  Biography = a.Author.Biography,
                                  IdTypeBook = new TypeBookResponse
                                  {
                                      IdTypeBook = a.Author.TypeBook.IdTypeBook,
                                      NameTypeBook = a.Author.TypeBook.NameTypeBook
                                  },
                                  Nationality = a.Author.Nationality
                              }).ToList(),
                    image = x.images.FirstOrDefault() != null ? x.images.FirstOrDefault()!.Url : string.Empty
                }).ToListAsync();
            return result;
        }
   
        public async Task<bool> DeleteEvaluation(DeleteEvaluationInput dto)
        {
            var user = await _authen.AuthenticationAsync(dto.token);
            if (user == null) return false;
            var evaluation = await _context.Evaluates.FirstOrDefaultAsync(x => x.IdEvaluate == dto.IdValuation);
            if (evaluation == null) return false; 
            _context.Evaluates.Remove(evaluation);
            await _context.SaveChangesAsync();
            return true; 

        }

        public async Task<List<GetHeaderbookResponse>> GetAllHeaderBooks()
        {
        
            var result = await _context.HeaderBooks
                .AsNoTracking()
                .Select(a => new GetHeaderbookResponse
                {
                    IdHeaderbook = a.IdHeaderBook,
                    NameBook = a.NameHeaderBook,
                    Describe = a.DescribeBook
                }).ToListAsync();

            return result; 
        }

        public async Task<List<BooksAndCommentsWithoutLogin>> findBook(string namebook)
        {
            var result = await _context.Books
            .AsNoTracking()
            .Where(x => x.HeaderBook.NameHeaderBook.ToLower().Contains(namebook.ToLower()))
            .Select(x => new BooksAndCommentsWithoutLogin
            {
                idBook = x.IdBook,
                nameBook = x.HeaderBook.NameHeaderBook,
                describe = x.HeaderBook.DescribeBook,
                image = x.images.Select(img => img.Url).FirstOrDefault() ?? string.Empty,
                valueOfbook = x.ValueOfBook,
                Evaluations = x.Evaluates.Select(c => new EvaluationDetails
                {
                    IdEvaluation = c.IdEvaluate,
                    IdReader = c.IdReader,
                    Comment = c.EvaComment,
                    Rating = c.EvaStar,
                    Create_Date = c.CreateDate
                }).ToList(),
                Authors = x.HeaderBook.bookWritings.Select(k => new AuthorResponse
                {
                    IdAuthor = k.IdAuthor,
                    NameAuthor = k.Author.NameAuthor,
                    IdTypeBook = new TypeBookResponse { IdTypeBook = k.Author.IdTypeBook, NameTypeBook = string.Empty },
                    Nationality = k.Author.Nationality,
                    Biography = k.Author.Biography,
                    UrlAvatar =  k.Author.Images.Select(x=>x.Url).FirstOrDefault() ?? string.Empty,
                }).ToList()
            }).ToListAsync();

            return result; 
        }

        public async Task<List<BooksAndComments>> getFavoriteBook(string idUser)
        {
            var result = await _context.FavoriteBooks
               .AsNoTracking()
               .Where(x=>x.IdReader == idUser)
               .Include(a => a.book).ThenInclude(x=>x.HeaderBook)
               .ThenInclude(x => x.bookWritings)
               .ThenInclude(c => c.Author)

               .Include(x=>x.book)
               .ThenInclude(a => a.Evaluates)

               .Include(x=>x.book)
               .ThenInclude(a => a.images)
               
               .Select(x => new BooksAndComments
               {
                   idBook = x.IdBook,
                   nameBook = x.book.HeaderBook.NameHeaderBook,
                   describe = x.book.HeaderBook.DescribeBook,
                   isLiked = _context.FavoriteBooks.Any(k => k.IdReader == idUser && k.IdBook == x.IdBook),
                   valueOfbook = x.book.ValueOfBook,
                   Evaluations = _context.Evaluates
                               .Where(a => a.IdBook == x.IdBook)
                               .Select(a => new EvaluationDetails
                               {
                                   IdEvaluation = a.IdEvaluate,
                                   IdReader = a.IdReader,
                                   Comment = a.EvaComment,
                                   Rating = a.EvaStar,
                                   Create_Date = a.CreateDate
                               }).ToList(),
                   Authors = x.book.HeaderBook.bookWritings
                             .Select(a => new AuthorResponse
                             {
                                 IdAuthor = a.IdAuthor,
                                 NameAuthor = a.Author.NameAuthor,
                                 Biography = a.Author.Biography,
                                 IdTypeBook = new TypeBookResponse
                                 {
                                     IdTypeBook = a.Author.TypeBook.IdTypeBook,
                                     NameTypeBook = a.Author.TypeBook.NameTypeBook
                                 },
                                 Nationality = a.Author.Nationality
                             }).ToList(),
                   image = x.book.images.FirstOrDefault(a=>a.IdBook == x.IdBook) != null ? x.book.images.FirstOrDefault()!.Url : string.Empty
               }).ToListAsync();
            return result; 
        }

        public async Task<List<BooksAndCommentsWithoutLogin>> getAllBooksInDetailById( string idbook)
        {
            var result = await _context.Books
          .AsNoTracking()
          .Where(x => x.IdBook == idbook)
          .Select(x => new BooksAndCommentsWithoutLogin
          {
              idBook = x.IdBook,
              nameBook = x.HeaderBook.NameHeaderBook,
              describe = x.HeaderBook.DescribeBook,
              image = x.images.Select(img => img.Url).FirstOrDefault() ?? string.Empty,
              valueOfbook = x.ValueOfBook,
              Evaluations = x.Evaluates.Select(c => new EvaluationDetails
              {
                  IdEvaluation = c.IdEvaluate,
                  IdReader = c.IdReader,
                  Comment = c.EvaComment,
                  Rating = c.EvaStar,
                  Create_Date = c.CreateDate
              }).ToList(),
              Authors = x.HeaderBook.bookWritings.Select(k => new AuthorResponse
              {
                  IdAuthor = k.IdAuthor,
                  NameAuthor = k.Author.NameAuthor,
                  IdTypeBook = new TypeBookResponse { IdTypeBook = k.Author.IdTypeBook, NameTypeBook = string.Empty },
                  Nationality = k.Author.Nationality,
                  Biography = k.Author.Biography,
                  UrlAvatar = k.Author.Images.Select(x=>x.Url).FirstOrDefault() ?? string.Empty,
              }).ToList()
          }).ToListAsync();

            return result;
        }

        public async Task<List<GetHeaderbookResponse>> GetAllHeaderBooksByTheBook(string idThebook)
        {
            var result = await _context.TheBooks.AsNoTracking()
                   .Where(x => x.IdTheBook == idThebook)
                   .Include(x=>x.Book).ThenInclude(x=>x.HeaderBook)
                   .Select(x=> new GetHeaderbookResponse
                   {
                        IdHeaderbook = x.Book.IdHeaderBook,
                        NameBook= x.Book.HeaderBook.NameHeaderBook, 
                        Describe = x.Book.HeaderBook.DescribeBook
                   })
                    .ToListAsync();
            return result; 
        }
    }
}
