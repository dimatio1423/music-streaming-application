using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class UserFavorite
{
    public int UserId { get; set; }

    public int SongId { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Song Song { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
