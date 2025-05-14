using ComputerStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductCreateDto productDto);
        Task UpdateProductAsync(ProductUpdateDto productDto);
        Task DeleteProductAsync(int id);
        Task ImportProductsAsync(List<ImportProductDto> importDtos);
        Task<decimal> CalculateDiscountAsync(List<BasketItemDto> basketItems);
    }
}