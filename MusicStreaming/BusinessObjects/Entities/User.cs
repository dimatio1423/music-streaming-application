using System;
using System.Collections.Generic;

namespace BusinessObjects.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? SubscriptionType { get; set; }

    public string? Role { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();

    public virtual ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();

    public virtual ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();

    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
