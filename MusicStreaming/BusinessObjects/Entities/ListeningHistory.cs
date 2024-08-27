using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class ListeningHistory
{
    public int HistoryId { get; set; }

    public int? UserId { get; set; }

    public int? SongId { get; set; }

    public DateTime? PlayedAt { get; set; }

    public TimeOnly? LastPauseTime { get; set; }

    public virtual Song? Song { get; set; }

    public virtual User? User { get; set; }
}
