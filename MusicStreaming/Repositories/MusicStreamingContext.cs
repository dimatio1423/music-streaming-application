using System;
using System.Collections.Generic;
using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories;

public partial class MusicStreamingContext : DbContext
{
    public MusicStreamingContext()
    {
    }

    public MusicStreamingContext(DbContextOptions<MusicStreamingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumSong> AlbumSongs { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<ArtistSong> ArtistSongs { get; set; }

    public virtual DbSet<ListeningHistory> ListeningHistories { get; set; }

    public virtual DbSet<OtpCode> OtpCodes { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnectionString"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__albums__B0E1DDB2E8074F5B");

            entity.ToTable("albums");

            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.ArtistId).HasColumnName("artist_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Genre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("genre");
            entity.Property(e => e.ImagePath)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums)
                .HasForeignKey(d => d.ArtistId)
                .HasConstraintName("FK__albums__artist_i__4D94879B");
        });

        modelBuilder.Entity<AlbumSong>(entity =>
        {
            entity.HasKey(e => new { e.AlbumId, e.SongId }).HasName("PK__album_so__6AB28753A2E7918B");

            entity.ToTable("album_songs");

            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.TrackNumber).HasColumnName("track_number");

            entity.HasOne(d => d.Album).WithMany(p => p.AlbumSongs)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__album_son__album__160F4887");

            entity.HasOne(d => d.Song).WithMany(p => p.AlbumSongs)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__album_son__song___17036CC0");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("PK__artists__6CD040011B91493D");

            entity.ToTable("artists");

            entity.Property(e => e.ArtistId).HasColumnName("artist_id");
            entity.Property(e => e.Bio)
                .HasColumnType("text")
                .HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImagePath)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Artists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__artists__user_id__2739D489");
        });

        modelBuilder.Entity<ArtistSong>(entity =>
        {
            entity.HasKey(e => new { e.ArtistId, e.SongId }).HasName("PK__artist_s__B6831AE0062DA2D3");

            entity.ToTable("artist_songs");

            entity.Property(e => e.ArtistId).HasColumnName("artist_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.IsOwner).HasColumnName("isOwner");
            entity.Property(e => e.RoleDescription)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_description");

            entity.HasOne(d => d.Artist).WithMany(p => p.ArtistSongs)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__artist_so__artis__282DF8C2");

            entity.HasOne(d => d.Song).WithMany(p => p.ArtistSongs)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__artist_so__song___29221CFB");
        });

        modelBuilder.Entity<ListeningHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__listenin__096AA2E91DB5D01B");

            entity.ToTable("listening_history");

            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.LastPauseTime).HasColumnName("last_pause_time");
            entity.Property(e => e.PlayedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("played_at");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Song).WithMany(p => p.ListeningHistories)
                .HasForeignKey(d => d.SongId)
                .HasConstraintName("FK__listening__song___4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.ListeningHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__listening__user___4F7CD00D");
        });

        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.HasKey(e => e.OptId).HasName("PK__OTPCode__3214EC0726F84CAA");

            entity.ToTable("otp_code");

            entity.Property(e => e.OptId).HasColumnName("opt_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsUsed).HasColumnName("isUsed");
            entity.Property(e => e.OptCode)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("opt_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.OtpCodes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OTPCode__Created__59FA5E80");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__playlist__FB9C1410FF82A294");

            entity.ToTable("playlists");

            entity.Property(e => e.PlaylistId).HasColumnName("playlist_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__playlists__user___52593CB8");
        });

        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.HasKey(e => new { e.PlaylistId, e.SongId }).HasName("PK__playlist__21CF4EF16A67C091");

            entity.ToTable("playlist_songs");

            entity.Property(e => e.PlaylistId).HasColumnName("playlist_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.PlaylistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__playlist___playl__5070F446");

            entity.HasOne(d => d.Song).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__playlist___song___5165187F");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK_RefreshToken");

            entity.ToTable("refresh_token");

            entity.Property(e => e.RefreshTokenId).HasColumnName("refresh_token_id");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expired_at");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PK__songs__A535AE1CF41F11FB");

            entity.ToTable("songs");

            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("file_path");
            entity.Property(e => e.ImagePath)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.Lyrics)
                .IsUnicode(false)
                .HasColumnName("lyrics");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B249DA5C5B803");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FB719034F");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164B9D97C16").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ImagePath)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValue("user")
                .HasColumnName("role");
            entity.Property(e => e.SubscriptionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("subscription_type");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.SongId }).HasName("PK__user_fav__63ED6DEE5BBAEEF6");

            entity.ToTable("user_favorites");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("added_at");

            entity.HasOne(d => d.Song).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_favo__song___5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_favo__user___5535A963");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.UserSubscriptionId).HasName("PK__UserSubs__D1FD777CFEA2F711");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Subscription).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSubscriptions_Subscriptions");

            entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSubscriptions_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
