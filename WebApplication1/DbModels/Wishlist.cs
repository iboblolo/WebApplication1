using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class Wishlist
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
