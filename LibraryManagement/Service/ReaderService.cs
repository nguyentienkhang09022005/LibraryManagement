using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Helpers.Constant;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LibraryManagement.Repository
{
    public class ReaderService : IReaderService
    {
        private readonly LibraryManagermentContext _context;
        private readonly IMapper _mapper;
        private readonly IAuthenService _account;
        private readonly IParameterService _parameterService;
        private readonly IUpLoadImageFileService _upLoadImageFileService;
        public ReaderService(LibraryManagermentContext contex, 
                                IMapper mapper, 
                                IAuthenService authen,
                                IParameterService parameterService,
                                IUpLoadImageFileService upLoadImageFileService)
        {
            _account = authen;
            _context = contex;
            _mapper = mapper;
            _parameterService = parameterService;
            _upLoadImageFileService = upLoadImageFileService;
        }

        // Hàm tạo Id độc giả
         public async Task<string> generateNextIdReaderAsync()
        {
            var nextID = await _context.Readers.OrderByDescending(id => id.IdReader).FirstOrDefaultAsync();

            int nextNumber = 1;

            if (nextID != null && nextID.IdReader.StartsWith("rd"))
            {
                string numberPart = nextID.IdReader.Substring(2);
                if (int.TryParse(numberPart, out int parsed))
                {
                    nextNumber = parsed + 1;
                }
            }
            return $"rd{nextNumber:D5}";
        }

        // Hàm thêm độc giả
        public async Task<ApiResponse<ReaderResponse>> addReaderAsync(ReaderCreationRequest request)
        {
            // Quy định tuổi độc giả
            int readerAge = DateTime.Now.Year - request.Dob.Year;
            if (request.Dob.Date > DateTime.Now.AddYears(-readerAge)) // Kiểm đã qua sinh nhật hay chưa
                readerAge--;

            int minAge = await _parameterService.getValueAsync("MinReaderAge");
            int maxAge = await _parameterService.getValueAsync("MaxReaderAge");
            if(readerAge < minAge || readerAge > maxAge) // Kiểm tra tuổi độc giả
            {
                return ApiResponse<ReaderResponse>.FailResponse($"Tuổi độc giả phải từ {minAge} đến {maxAge} tuổi", 400);
            }

            // Validate số điện thoại
            if (!string.IsNullOrEmpty(request.Phone))
            {
                if (!Regex.IsMatch(request.Phone, @"^\d{10,12}$"))
                    return ApiResponse<ReaderResponse>.FailResponse("Số điện thoại phải gồm từ 10 đến 12 chữ số", 400);
            }

            // Chuỗi url ảnh từ cloudinary
            string imageUrl = "https://res.cloudinary.com/df41zs8il/image/upload/v1750223521/default-avatar-icon-of-social-media-user-vector_a3a2de.jpg";

            var newReader = new Reader
            {
                IdReader = await generateNextIdReaderAsync(),
                IdTypeReader = request.IdTypeReader,
                NameReader = request.NameReader,
                Sex = request.Sex,
                Address = request.Address,
                Email = request.Email,
                Dob = DateTime.SpecifyKind(request.Dob, DateTimeKind.Utc),
                Phone = request.Phone,
                CreateDate = DateTime.UtcNow,
                ReaderPassword = BCrypt.Net.BCrypt.HashPassword(request.ReaderPassword),
                RoleName = ConstantRoles.Reader
            };

            _context.Readers.Add(newReader);
            await _context.SaveChangesAsync();

            // Lưu avatar vào bảng image nếu có
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var image = new Image
                {
                    IdReader = newReader.IdReader,
                    Url = imageUrl,
                };
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
            }

            var typeReader = await _context.TypeReaders
                .Where(tr => tr.IdTypeReader == newReader.IdTypeReader)
                .Select(tr => new TypeReaderResponse
                {
                    idTypeReader = tr.IdTypeReader,
                    NameTypeReader = tr.NameTypeReader
                })
                .FirstOrDefaultAsync();


            var readerResponse = new ReaderResponse
            {
                IdReader = newReader.IdReader,
                IdTypeReader = typeReader,
                NameReader = newReader.NameReader,
                Sex = newReader.Sex,
                Address = newReader.Address,
                Email = newReader.Email,
                Dob = newReader.Dob,
                Phone = newReader.Phone,
                CreateDate = newReader.CreateDate,
                TotalDebt = newReader.TotalDebt,
                UrlAvatar = imageUrl
            };
            return ApiResponse<ReaderResponse>.SuccessResponse("Thêm độc giả thành công", 201, readerResponse);
        }

        // Hàm lấy danh sách độc giả
        public async Task<List<ReaderResponse>> getAllReaderAsync()
        {
            var listReader = await _context.Readers
                .AsNoTracking()
                .Select(readerInf => new ReaderResponse
                {
                    IdReader = readerInf.IdReader,
                    NameReader = readerInf.NameReader!,
                    Sex = readerInf.Sex!,
                    Address = readerInf.Address!,
                    Email = readerInf.Email!,
                    Dob = readerInf.Dob,
                    Phone = readerInf.Phone!,
                    CreateDate = readerInf.CreateDate,
                    TotalDebt = readerInf.TotalDebt,
                    Role = readerInf.Role.RoleName,
                    UrlAvatar = readerInf.Images
                    .OrderBy(a => a.IdReader) 
                    .Select(x => x.Url)
                    .FirstOrDefault(),
                    IdTypeReader = readerInf.TypeReader != null
                        ? new TypeReaderResponse
                        {
                            idTypeReader = readerInf.TypeReader.IdTypeReader,
                            NameTypeReader = readerInf.TypeReader.NameTypeReader
                        }
                        : null,
                    
                }).ToListAsync();
            return listReader;
        }


        // Hàm sửa độc giả
        public async Task<ApiResponse<ReaderResponse>> updateReaderAsync(ReaderUpdateRequest request, string idReader)
        {
            // Kiểm tra tồn tại
            var readerExists = await _context.Readers.AnyAsync(r => r.IdReader == idReader);
            if (!readerExists)
                return ApiResponse<ReaderResponse>.FailResponse("Không tìm thấy độc giả", 404);

            // Validate DOB và tuổi
            if (request.Dob.HasValue)
            {
                int readerAge = DateTime.Now.Year - request.Dob.Value.Year;
                if (request.Dob.Value.Date > DateTime.Now.AddYears(-readerAge)) readerAge--;

                int minAge = await _parameterService.getValueAsync("MinReaderAge");
                int maxAge = await _parameterService.getValueAsync("MaxReaderAge");
                if (readerAge < minAge || readerAge > maxAge)
                    return ApiResponse<ReaderResponse>.FailResponse($"Tuổi độc giả phải từ {minAge} đến {maxAge} tuổi", 400);
            }

            // Validate số điện thoại
            if (!string.IsNullOrEmpty(request.Phone))
            {
                if (!Regex.IsMatch(request.Phone, @"^\d{10,12}$"))
                    return ApiResponse<ReaderResponse>.FailResponse("Số điện thoại phải gồm từ 10 đến 12 chữ số", 400);
            }

            // Kiểm tra giá trị Sex có hợp lệ hay không
            if (!string.IsNullOrEmpty(request.Sex))
            {
                var allowed = new[] { "Nam", "Nữ" };
                if (!allowed.Contains(request.Sex))
                    return ApiResponse<ReaderResponse>.FailResponse("Giới tính không hợp lệ", 400);
            }

            // Xử lý hash password nếu có
            string? hashedPassword = null;
            if (!string.IsNullOrEmpty(request.ReaderPassword))
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.ReaderPassword);

            // Dùng ExecuteUpdateAsync để update trực tiếp
         

            await _context.Readers
                .AsNoTracking()
                .Where(x=>x.IdReader == idReader)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.NameReader, r => !string.IsNullOrEmpty(request.NameReader) ? request.NameReader : r.NameReader)
                .SetProperty(r => r.Dob, r => request.Dob.HasValue ? DateTime.SpecifyKind(request.Dob.Value, DateTimeKind.Utc) : r.Dob)
                .SetProperty(r => r.IdTypeReader, r => request.IdTypeReader.HasValue && request.IdTypeReader.Value != Guid.Empty ? request.IdTypeReader.Value : r.IdTypeReader)
                .SetProperty(r => r.Sex, r => !string.IsNullOrEmpty(request.Sex) ? request.Sex : r.Sex)
                .SetProperty(r => r.Address, r => !string.IsNullOrEmpty(request.Address) ? request.Address : r.Address)
                .SetProperty(r => r.Email, r => !string.IsNullOrEmpty(request.Email) ? request.Email : r.Email)
                .SetProperty(r => r.Phone, r => !string.IsNullOrEmpty(request.Phone) ? request.Phone : r.Phone)
                .SetProperty(r => r.ReaderPassword, r => !string.IsNullOrEmpty(request.ReaderPassword) ? hashedPassword : r.ReaderPassword)
            );

            if (request.AvatarImage != null)
            {
                string imageUrl = await _upLoadImageFileService.UploadImageAsync(request.AvatarImage);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _context.Images.AsNoTracking()
                            .Where(x => x.IdReader == idReader)
                            .ExecuteUpdateAsync(setter =>
                            setter.SetProperty(a => a.Url, imageUrl));
                }
            }
            var readerResponse = await _context.Readers.AsNoTracking()
                   .Where(x => x.IdReader == idReader)
                   .Select(x => new ReaderResponse
                   {
                       IdReader = x.IdReader,
                       IdTypeReader = new TypeReaderResponse { idTypeReader = x.IdTypeReader, NameTypeReader = x.TypeReader.NameTypeReader },
                       NameReader = x.NameReader,
                       Sex = x.Sex,
                       Address = x.Address,
                       Email = x.Email,
                       Dob = x.Dob,
                       Phone = x.Phone,
                       CreateDate = x.CreateDate,
                       TotalDebt = x.TotalDebt,
                       UrlAvatar = x.Images.Select(x=>x.Url).FirstOrDefault()
                   }).FirstOrDefaultAsync();
            return ApiResponse<ReaderResponse>.SuccessResponse("Thay đổi thông tin độc giả thành công", 200, readerResponse);
        }


        // Hàm xóa độc giả
        public async Task<ApiResponse<string>> deleteReaderAsync(string idReader)
        {
            var deleteReader = await _context.Readers.FirstOrDefaultAsync(reader => reader.IdReader == idReader);
            if (deleteReader == null)
            {
                return ApiResponse<string>.FailResponse("Không tìm thấy độc giả", 404);
            }
            _context.Readers.Remove(deleteReader);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse("Đã xóa độc giả", 200, "");
        }

        public async Task<FindReaderOutputDto> findReaderAsync(string dto)
        {
      
            var listReader = await _context.Readers.AsNoTracking().Where(x => x.Email == dto).Select(a => new FindReaderOutputDto
            {
                phone = a.Phone!, 
                Email = a.Email!,
                password = a.ReaderPassword,
                DateCreate = a.CreateDate
            }
            ).FirstOrDefaultAsync();
            return listReader!;
        }

        public async Task<FindReaderOutputDto> findReaderInputAsync(string idReader)
        {
            var listReader = await _context.Readers.AsNoTracking().Where(x => x.IdReader == idReader).Select(a => new FindReaderOutputDto
            {
                phone = a.Phone!,
                Email = a.Email!,
                password = a.ReaderPassword,
                DateCreate = a.CreateDate
            }
          ).FirstOrDefaultAsync();
            return listReader!;
        }
    }
}
