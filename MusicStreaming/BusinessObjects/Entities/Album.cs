using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Album
{
    public int AlbumId { get; set; }

    public int? ArtistId { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public string? Genre { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ImagePath { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<AlbumSong> AlbumSongs { get; set; } = new List<AlbumSong>();

    public virtual Artist? Artist { get; set; }
}
