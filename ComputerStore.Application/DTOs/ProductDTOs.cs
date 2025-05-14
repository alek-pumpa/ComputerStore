using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<int> CategoryIds { get; set; } = new();
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<int> CategoryIds { get; set; } = new();
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public int Id { get; set; }
    }

    public class ImportProductDto
    {
        public string Name { get; set; } = null!;
        public List<string> Categories { get; set; } = new();
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}