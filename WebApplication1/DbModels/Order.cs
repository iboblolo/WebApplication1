using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public decimal Cost { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? Date { get; set; }

    public virtual ICollection<CartToOrder> CartToOrders { get; set; } = new List<CartToOrder>();
}
