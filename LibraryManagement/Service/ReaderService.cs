using AutoMapper;
using LibraryManagement.Data;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Helpers;
using LibraryManagement.Models;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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

            // Chuỗi url ảnh từ cloudinary
            string imageUrl = null;
            if (request.AvatarImage != null)
            {
                imageUrl = await _upLoadImageFileService.UploadImageAsync(request.AvatarImage);
            }

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
                ReaderUsername = request.Email,
                ReaderPassword = BCrypt.Net.BCrypt.HashPassword(request.ReaderPassword),
                RoleName = AppRoles.Reader
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
                ReaderAccount = newReader.ReaderUsername,
                TotalDebt = newReader.TotalDebt,
                UrlAvatar = imageUrl
            };
            return ApiResponse<ReaderResponse>.SuccessResponse("Thêm độc giả thành công", 201, readerResponse);
        }

        // Hàm lấy danh sách độc giả
        public async Task<List<ReaderResponse>> getAllReaderAsync()
        {
            var listReader = await _context.Readers
                .Include(r => r.Images)
                .Include(r => r.TypeReader)
                .ToListAsync();

            var readerResponse = new List<ReaderResponse>();

            foreach (var readerInf in listReader)
            {
                var response = new ReaderResponse
                {
                    IdReader = readerInf.IdReader,
                    NameReader = readerInf.NameReader!,
                    Sex = readerInf.Sex!,
                    Address = readerInf.Address!,
                    Email = readerInf.Email!,
                    Dob = readerInf.Dob,
                    Phone = readerInf.Phone!,
                    CreateDate = readerInf.CreateDate,
                    ReaderAccount = readerInf.Email!,
                    TotalDebt = readerInf.TotalDebt,
                    UrlAvatar = readerInf.Images?.FirstOrDefault()?.Url,
                    IdTypeReader = readerInf.TypeReader != null
                        ? new TypeReaderResponse
                        {
                            idTypeReader = readerInf.TypeReader.IdTypeReader,
                            NameTypeReader = readerInf.TypeReader.NameTypeReader
                        }
                        : null
                };
                readerResponse.Add(response);
            }
            return readerResponse;
        }


        // Hàm sửa độc giả
        public async Task<ApiResponse<ReaderResponse>> updateReaderAsync(ReaderUpdateRequest request, string idReader)
        {
            var updateReader = await _context.Readers.FirstOrDefaultAsync(reader => reader.IdReader == idReader);
            if (updateReader == null)
            {
                return ApiResponse<ReaderResponse>.FailResponse("Không tìm thấy độc giả", 404);
            }
            // Quy định tuổi độc giả
            int readerAge = DateTime.Now.Year - request.Dob.Year;
            if (request.Dob.Date > DateTime.Now.AddYears(-readerAge)) // Kiểm đã qua sinh nhật hay chưa
                readerAge--;

            int minAge = await _parameterService.getValueAsync("MinReaderAge");
            int maxAge = await _parameterService.getValueAsync("MaxReaderAge");
            if (readerAge < minAge || readerAge > maxAge) // Kiểm tra tuổi độc giả
            {
                return ApiResponse<ReaderResponse>.FailResponse($"Tuổi độc giả phải từ {minAge} đến {maxAge} tuổi", 400);
            }

            // Chuỗi url ảnh từ cloudinary
            string imageUrl = null;
            if (request.AvatarImage != null)
            {
                imageUrl = await _upLoadImageFileService.UploadImageAsync(request.AvatarImage);
            }

            updateReader.IdTypeReader = request.IdTypeReader;
            updateReader.NameReader = request.NameReader;
            updateReader.Sex = request.Sex;
            updateReader.Address = request.Address;
            updateReader.Email = request.Email;
            updateReader.Dob = DateTime.SpecifyKind(request.Dob, DateTimeKind.Utc);
            updateReader.Phone = request.Phone;
            updateReader.ReaderUsername = request.Email;
            if (!string.IsNullOrEmpty(request.ReaderPassword))
            {
                updateReader.ReaderPassword = BCrypt.Net.BCrypt.HashPassword(request.ReaderPassword);
            }

            // Cập nhật hoặc thêm mới ảnh nếu có ảnh mới
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var existingAvatar = await _context.Images.FirstOrDefaultAsync(av => av.IdReader == updateReader.IdReader);
                if (existingAvatar != null)
                {
                    existingAvatar.Url = imageUrl;
                    _context.Images.Update(existingAvatar);
                }
                else
                {
                    var image = new Image
                    {
                        IdReader = updateReader.IdReader,
                        Url = imageUrl,
                    };
                    _context.Images.Add(image);
                }
            }

            var typeReader = await _context.TypeReaders
                .Where(tr => tr.IdTypeReader == updateReader.IdTypeReader)
                .Select(tr => new TypeReaderResponse
                {
                    idTypeReader = tr.IdTypeReader,
                    NameTypeReader = tr.NameTypeReader
                })
                .FirstOrDefaultAsync();

            var readerResponse = new ReaderResponse
            {
                IdReader = updateReader.IdReader,
                IdTypeReader = typeReader,
                NameReader = updateReader.NameReader,
                Sex = updateReader.Sex,
                Address = updateReader.Address,
                Email = updateReader.Email,
                Dob = updateReader.Dob,
                Phone = updateReader.Phone,
                CreateDate = updateReader.CreateDate,
                ReaderAccount = updateReader.ReaderUsername,
                TotalDebt = updateReader.TotalDebt,
                UrlAvatar = imageUrl
            };
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
      
            var listReader = await _context.Readers.Where(x => x.ReaderUsername == dto).Select(a => new FindReaderOutputDto
            {
                username = a.ReaderUsername,
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
