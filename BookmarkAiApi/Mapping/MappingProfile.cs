using AutoMapper;
using BookmarkAiApi.Dtos;
using BookmarkAiApi.Models;

namespace BookmarkAiApi.Mapping;

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