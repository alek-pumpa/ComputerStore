using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ComputerStore.Application.DTOs;
using ComputerStore.Application.Services;
using ComputerStore.Domain.Entities;
using ComputerStore.Tests.Unit.Common;
using Moq;
using Xunit;

namespace ComputerStore.Tests.Unit.Services
{
    public class ProductServiceTests
    {
        private readonly MockRepository<Product> _mockProductRepo;
        private readonly MockRepository<Category> _mockCategoryRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockProductRepo = new MockRepository<Product>();
            _mockCategoryRepo = new MockRepository<Category>();
            _mockMapper = new Mock<IMapper>();
            _service = new ProductService(
                _mockProductRepo,
                _mockCategoryRepo,
                _mockMapper.Object);
        }

        [Fact]
        public async Task CreateProductAsync_ValidInput_CreatesProduct()
        {
            var category = new Category { Id = 1, Name = "CPU" };
            _mockCategoryRepo.SeedData(new List<Category> { category });

            var productDto = new ProductCreateDto
            {
                Name = "Intel Core i9",
                Price = 499.99m,
                CategoryIds = new List<int> { 1 }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Intel Core i9",
                Price = 499.99m,
                Categories = new List<Category> { category }
            };

            _mockMapper.Setup(m => m.Map<Product>(productDto))
                .Returns(new Product { Name = productDto.Name, Price = productDto.Price });
            _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(new ProductDto { Id = 1, Name = product.Name, Price = product.Price });

            var result = await _service.CreateProductAsync(productDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Intel Core i9", result.Name);
            Assert.Equal(1, _mockProductRepo.Count);
        }

        [Fact]
        public async Task CalculateDiscountAsync_TwoProductsSameCategory_Applies5PercentDiscount()
        {
            var category = new Category { Id = 1, Name = "CPU" };
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Intel Core i9", Price = 500, Quantity = 10, Categories = new List<Category> { category } },
                new Product { Id = 2, Name = "AMD Ryzen 9", Price = 450, Quantity = 8, Categories = new List<Category> { category } }
            };
            _mockProductRepo.SeedData(products);

            var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ProductId = 1, Quantity = 2 },
                new BasketItemDto { ProductId = 2, Quantity = 1 }
            };


            var discount = await _service.CalculateDiscountAsync(basketItems);


            Assert.Equal(25m, discount); 
        }

        [Fact]
        public async Task ImportProductsAsync_NewProductAndCategory_CreatesBoth()
        {
            var importDtos = new List<ImportProductDto>
            {
                new ImportProductDto
                {
                    Name = "NVIDIA RTX 4090",
                    Categories = new List<string> { "GPU" },
                    Price = 1599.99m,
                    Quantity = 5
                }
            };

            await _service.ImportProductsAsync(importDtos);

            Assert.Equal(1, _mockProductRepo.Count);
            Assert.Equal(1, _mockCategoryRepo.Count);
            var product = (await _mockProductRepo.GetAllAsync()).First();
            var category = (await _mockCategoryRepo.GetAllAsync()).First();
            Assert.Equal("NVIDIA RTX 4090", product.Name);
            Assert.Equal("GPU", category.Name);
            Assert.Equal(1599.99m, product.Price);
            Assert.Equal(5, product.Quantity);
        }
    }
}