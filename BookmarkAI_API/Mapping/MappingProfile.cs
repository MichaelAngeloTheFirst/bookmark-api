using BookmarkAI_API.Dtos;
using BookmarkAI_API.Models;
using AutoMapper;

namespace BookmarkAI_API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bookmark, BookmarkResponseDto>()
            .ForMember(dest => dest.Tags, opt =>
                opt.MapFrom(src => src.BookmarkTags.Select(ut => ut.Tag.Name).ToList()));

        CreateMap<Tag, TagDto>();
        CreateMap<Collection, CollectionDto>();
    }
}