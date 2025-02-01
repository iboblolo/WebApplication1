using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class CartToOrder
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int OrderId { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
