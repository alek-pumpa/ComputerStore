using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using ComputerStore.Application.DTOs;
using ComputerStore.Domain.Entities;

namespace ComputerStore.Tests.Unit.Common
{
    public static class MockMapper
    {
        public static IMapper Create()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>();
                cfg.CreateMap<CategoryCreateDto, Category>();
                cfg.CreateMap<CategoryUpdateDto, Category>();

                cfg.CreateMap<Product, ProductDto>()
                    .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories.Select(c => c.Id)));
                cfg.CreateMap<ProductCreateDto, Product>();
                cfg.CreateMap<ProductUpdateDto, Product>();
            });

            return mapperConfiguration.CreateMapper();
        }
    }
}