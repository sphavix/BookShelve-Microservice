using AutoMapper;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.Author;
using BookShelve.Api.Models.Book;
using BookShelve.Api.Models.User;

namespace BookShelve.Api.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReadAuthorDto, Author>().ReverseMap();
            CreateMap<UpdateAuthorDto, Author>().ReverseMap();
            CreateMap<CreateAuthorDto, Author>().ReverseMap();

            CreateMap<Book, ReadBookDto>()
                .ForMember(b => b.AuthorName, src => src.MapFrom(m => $"{m.Author.FirstName} {m.Author.LastName}")) // Map from source to Dest, Conc the fields from entity to Dto
                .ReverseMap();

            CreateMap<Book, BookDetailsDto>()
                .ForMember(b => b.AuthorName, src => src.MapFrom(m => $"{m.Author.FirstName} {m.Author.LastName}")) // Conc the fields from entity to Dto
                .ReverseMap();
            CreateMap<CreateBookDto, Book>().ReverseMap();
            CreateMap<Book, UpdateBookDto>().ReverseMap();

            CreateMap<ApplicationUser, RegisterUserDto>().ReverseMap();


        }
    }
}
