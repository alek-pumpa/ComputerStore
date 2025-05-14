using AutoMapper;
using ComputerStore.Application.DTOs;
using ComputerStore.Application.Interfaces;
using ComputerStore.Domain.Common;
using ComputerStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            foreach (var categoryId in productDto.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

                product.Categories.Add(category);
            }

            await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(ProductUpdateDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");

            _mapper.Map(productDto, product);
            product.Categories.Clear();

            foreach (var categoryId in productDto.CategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

                product.Categories.Add(category);
            }

            _productRepository.Update(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            _productRepository.Delete(product);
        }

        public async Task ImportProductsAsync(List<ImportProductDto> importDtos)
        {
            foreach (var dto in importDtos)
            {
                var product = (await _productRepository.GetAllAsync())
                    .FirstOrDefault(p => p.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                {
                    product = new Product
                    {
                        Name = dto.Name,
                        Price = dto.Price,
                        Quantity = dto.Quantity
                    };
                    await _productRepository.AddAsync(product);
                }
                else
                {
                    product.Price = dto.Price;
                    product.Quantity = dto.Quantity;
                    _productRepository.Update(product);
                }

                foreach (var categoryName in dto.Categories)
                {
                    var trimmedName = categoryName.Trim();
                    var category = (await _categoryRepository.GetAllAsync())
                        .FirstOrDefault(c => c.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase));

                    if (category == null)
                    {
                        category = new Category { Name = trimmedName };
                        await _categoryRepository.AddAsync(category);
                    }

                    if (!product.Categories.Any(c => c.Id == category.Id))
                    {
                        product.Categories.Add(category);
                    }
                }
            }
        }

        public async Task<decimal> CalculateDiscountAsync(List<BasketItemDto> basketItems)
        {
            if (basketItems.Count == 0)
                return 0;

            foreach (var item in basketItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");

                if (product.Quantity < item.Quantity)
                    throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {product.Quantity}, Requested: {item.Quantity}");
            }

            var products = (await _productRepository.GetAllAsync())
                .Where(p => basketItems.Any(b => b.ProductId == p.Id))
                .ToList();

            var categoryTotals = new Dictionary<int, int>();
            foreach (var product in products)
            {
                foreach (var category in product.Categories)
                {
                    if (!categoryTotals.ContainsKey(category.Id))
                        categoryTotals[category.Id] = 0;

                    var basketItem = basketItems.First(b => b.ProductId == product.Id);
                    categoryTotals[category.Id] += basketItem.Quantity;
                }
            }

            decimal discount = 0;
            foreach (var product in products)
            {
                var basketItem = basketItems.First(b => b.ProductId == product.Id);

                if (basketItem.Quantity == 1)
                    continue;

                foreach (var category in product.Categories)
                {
                    if (categoryTotals[category.Id] > 1)
                    {
                        discount += product.Price * 0.05m;
                    }
                }
            }

            return discount;
        }
    }
}