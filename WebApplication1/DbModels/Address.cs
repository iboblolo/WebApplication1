using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class Addresses
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
