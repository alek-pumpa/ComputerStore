using ComputerStore.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerStore.Domain.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
