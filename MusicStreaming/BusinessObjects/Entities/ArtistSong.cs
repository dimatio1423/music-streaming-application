using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class ArtistSong
{
    public int ArtistId { get; set; }

    public int SongId { get; set; }

    public string? RoleDescription { get; set; }

    public bool? IsOwner { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
