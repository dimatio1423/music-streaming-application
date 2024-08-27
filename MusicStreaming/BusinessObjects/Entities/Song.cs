using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class Song
{
    public int SongId { get; set; }

    public string Title { get; set; } = null!;

    public TimeOnly? Duration { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? Status { get; set; }

    public string? Lyrics { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<AlbumSong> AlbumSongs { get; set; } = new List<AlbumSong>();

    public virtual ICollection<ArtistSong> ArtistSongs { get; set; } = new List<ArtistSong>();

    public virtual ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
