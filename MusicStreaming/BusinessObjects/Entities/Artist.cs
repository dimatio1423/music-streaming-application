using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Artist
{
    public int ArtistId { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ImagePath { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<ArtistSong> ArtistSongs { get; set; } = new List<ArtistSong>();

    public virtual User? User { get; set; }
}
