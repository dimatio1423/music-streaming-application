using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class AlbumSong
{
    public int AlbumId { get; set; }

    public int SongId { get; set; }

    public int? TrackNumber { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
