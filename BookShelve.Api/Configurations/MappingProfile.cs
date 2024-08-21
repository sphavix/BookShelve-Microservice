using AutoMapper;
using BookShelve.Api.Domain.Entities;
using BookShelve.Api.Models.Author;

namespace BookShelve.Api.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReadAuthorDto, Author>().ReverseMap();
            CreateMap<UpdateAuthorDto, Author>().ReverseMap();
            CreateMap<CreateAuthorDto, Author>().ReverseMap();
        }
    }
}
