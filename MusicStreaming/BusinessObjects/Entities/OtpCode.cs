using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class OtpCode
{
    public int OptId { get; set; }

    public string OptCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
