using AutoMapper;
using ComputerStore.Domain.Entities;
using ComputerStore.Application.DTOs;

namespace ComputerStore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<Product, ImportProductDto>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name).ToList()));
            CreateMap<ImportProductDto, Product>();

        }
    }
}
