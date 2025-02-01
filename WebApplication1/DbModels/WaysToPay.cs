using System;
using System.Collections.Generic;

namespace WebApplication1.DbModels;

public partial class WaysToPay
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string WayToPay { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
