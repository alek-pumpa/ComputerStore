using AutoMapper;
using ComputerStore.Application.DTOs;
using ComputerStore.Application.Services;
using ComputerStore.Domain.Common;
using ComputerStore.Domain.Entities;
using ComputerStore.Tests.Unit.Common;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Tests.Unit.Services
{
    public class CategoryServiceTests
    {
        private readonly MockRepository<Category> _mockRepo;
        private readonly IMapper _mapper;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepo = new MockRepository<Category>();
            _mapper = MockMapper.Create();
            _service = new CategoryService(_mockRepo, _mapper);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "CPU", Description = "Processors" },
                new Category { Id = 2, Name = "GPU", Description = "Graphics cards" }
            };
            _mockRepo.SeedData(categories);

            var result = await _service.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "CPU");
            Assert.Contains(result, c => c.Name == "GPU");
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidInput_CreatesCategory()
        {
            var categoryDto = new CategoryCreateDto
            {
                Name = "SSD",
                Description = "Solid State Drives"
            };

            var result = await _service.CreateCategoryAsync(categoryDto);

            Assert.NotNull(result);
            Assert.Equal("SSD", result.Name);
            Assert.Equal("Solid State Drives", result.Description);
            Assert.Equal(1, _mockRepo.Count);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ExistingCategory_UpdatesSuccessfully()
        {
            var existingCategory = new Category { Id = 1, Name = "HDD", Description = "Hard Disk Drives" };
            _mockRepo.SeedData(new List<Category> { existingCategory });

            var updateDto = new CategoryUpdateDto
            {
                Id = 1,
                Name = "SSD",
                Description = "Solid State Drives"
            };

            await _service.UpdateCategoryAsync(updateDto);

            var updatedCategory = (await _mockRepo.GetAllAsync()).First();
            Assert.Equal("SSD", updatedCategory.Name);
            Assert.Equal("Solid State Drives", updatedCategory.Description);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ExistingCategory_DeletesSuccessfully()
        {            
            var category = new Category { Id = 1, Name = "CPU" };
            _mockRepo.SeedData(new List<Category> { category });

            await _service.DeleteCategoryAsync(1);

            Assert.Equal(0, _mockRepo.Count);
        }
    }
}