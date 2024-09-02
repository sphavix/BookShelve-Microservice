using AutoMapper;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.Author;
using BookShelve.Api.Models.Book;

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
                .ForMember(b => b.AuthorName, src => src.MapFrom(m => $"{m.Author.FirstName} {m.Author.LastName}")) // Conc the fields from entity to Dto
                .ReverseMap();
        }
    }
}
