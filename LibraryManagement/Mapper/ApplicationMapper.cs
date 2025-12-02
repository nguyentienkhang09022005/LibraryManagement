using AutoMapper;
using LibraryManagement.Dto.Request;
using LibraryManagement.Dto.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            // Mapper Author
            CreateMap<AuthorCreationRequest, Author>();
            CreateMap<Author, AuthorResponse>()
                .ForMember(dest => dest.IdTypeBook, opt => opt.Ignore())
                .ForMember(dest => dest.UrlAvatar, opt => opt.Ignore());


            // Mapper TypeReader
            CreateMap<TypeReaderRequest, TypeReader>();
            CreateMap<TypeReader, TypeReaderResponse>();

            // Mapper TypeBook 
            CreateMap<TypeBookRequest, TypeBook>();
            CreateMap<TypeBook, TypeBookResponse>();

            // Mapper Role
            CreateMap<RoleRequest, Role>();
            CreateMap<Role, RoleResponse>();

            // Mapper Parameter
            CreateMap<ParameterRequest, Parameter>();
            CreateMap<Parameter, ParameterResponse>();
        }
    }
}
