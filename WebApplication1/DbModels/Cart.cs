using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class Cart
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public int Count { get; set; }

    public bool IsOrdered { get; set; }

    public virtual ICollection<CartToOrder> CartToOrders { get; set; } = new List<CartToOrder>();

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
