using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class Product
{
    public int Id { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public string? Image { get; set; }
}
