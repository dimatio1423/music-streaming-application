using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public string RefreshToken1 { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
