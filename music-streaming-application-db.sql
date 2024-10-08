USE [master]
GO
/****** Object:  Database [music_streaming]    Script Date: 9/3/2024 12:08:21 AM ******/
CREATE DATABASE [music_streaming]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'music_streaming', FILENAME = N'C:\Program Files\Microsoft SQL Server 2019\MSSQL15.SQLEXPRESS01\MSSQL\DATA\music_streaming.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'music_streaming_log', FILENAME = N'C:\Program Files\Microsoft SQL Server 2019\MSSQL15.SQLEXPRESS01\MSSQL\DATA\music_streaming_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [music_streaming] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [music_streaming].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [music_streaming] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [music_streaming] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [music_streaming] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [music_streaming] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [music_streaming] SET ARITHABORT OFF 
GO
ALTER DATABASE [music_streaming] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [music_streaming] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [music_streaming] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [music_streaming] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [music_streaming] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [music_streaming] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [music_streaming] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [music_streaming] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [music_streaming] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [music_streaming] SET  ENABLE_BROKER 
GO
ALTER DATABASE [music_streaming] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [music_streaming] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [music_streaming] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [music_streaming] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [music_streaming] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [music_streaming] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [music_streaming] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [music_streaming] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [music_streaming] SET  MULTI_USER 
GO
ALTER DATABASE [music_streaming] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [music_streaming] SET DB_CHAINING OFF 
GO
ALTER DATABASE [music_streaming] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [music_streaming] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [music_streaming] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [music_streaming] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [music_streaming] SET QUERY_STORE = OFF
GO
USE [music_streaming]
GO
/****** Object:  Table [dbo].[album_songs]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[album_songs](
	[album_id] [int] NOT NULL,
	[song_id] [int] NOT NULL,
	[track_number] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[album_id] ASC,
	[song_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[albums]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[albums](
	[album_id] [int] IDENTITY(1,1) NOT NULL,
	[artist_id] [int] NULL,
	[title] [nvarchar](max) NOT NULL,
	[release_date] [date] NULL,
	[genre] [varchar](255) NULL,
	[created_at] [datetime] NULL,
	[image_path] [varchar](max) NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK__albums__B0E1DDB2E8074F5B] PRIMARY KEY CLUSTERED 
(
	[album_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[artist_songs]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[artist_songs](
	[artist_id] [int] NOT NULL,
	[song_id] [int] NOT NULL,
	[role_description] [varchar](50) NULL,
	[isOwner] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[artist_id] ASC,
	[song_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[artists]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[artists](
	[artist_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[bio] [text] NULL,
	[created_at] [datetime] NULL,
	[image_path] [varchar](max) NULL,
	[user_id] [int] NULL,
 CONSTRAINT [PK__artists__6CD040011B91493D] PRIMARY KEY CLUSTERED 
(
	[artist_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[listening_history]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[listening_history](
	[history_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[song_id] [int] NULL,
	[played_at] [datetime] NULL,
	[last_pause_time] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[history_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[otp_code]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[otp_code](
	[opt_id] [int] IDENTITY(1,1) NOT NULL,
	[opt_code] [char](6) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[isUsed] [bit] NOT NULL,
	[user_id] [int] NOT NULL,
 CONSTRAINT [PK__OTPCode__3214EC0726F84CAA] PRIMARY KEY CLUSTERED 
(
	[opt_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[PayementDate] [datetime] NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[user_id] [int] NOT NULL,
	[SubscriptionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[playlist_songs]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[playlist_songs](
	[playlist_id] [int] NOT NULL,
	[song_id] [int] NOT NULL,
	[added_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[playlist_id] ASC,
	[song_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[playlists]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[playlists](
	[playlist_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[title] [nvarchar](max) NOT NULL,
	[created_at] [datetime] NULL,
 CONSTRAINT [PK__playlist__FB9C1410FF82A294] PRIMARY KEY CLUSTERED 
(
	[playlist_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[refresh_token]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[refresh_token](
	[refresh_token_id] [int] IDENTITY(1,1) NOT NULL,
	[refresh_token] [varchar](128) NOT NULL,
	[expired_at] [datetime] NOT NULL,
	[user_id] [int] NOT NULL,
 CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED 
(
	[refresh_token_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[songs]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[songs](
	[song_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](255) NOT NULL,
	[duration] [time](7) NULL,
	[file_path] [varchar](255) NOT NULL,
	[created_at] [datetime] NULL,
	[status] [varchar](50) NULL,
	[lyrics] [varchar](max) NULL,
	[image_path] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[song_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subscriptions]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscriptions](
	[SubscriptionId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[DurationInDays] [int] NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Status] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[SubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_favorites]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_favorites](
	[user_id] [int] NOT NULL,
	[song_id] [int] NOT NULL,
	[added_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[song_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_queues]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_queues](
	[user_id] [int] NOT NULL,
	[song_id] [int] NOT NULL,
	[added_at] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](255) NOT NULL,
	[email] [varchar](255) NOT NULL,
	[password] [varchar](255) NOT NULL,
	[created_at] [datetime] NULL,
	[subscription_type] [varchar](50) NULL,
	[role] [nvarchar](max) NULL,
	[image_path] [varchar](max) NULL,
	[status] [varchar](50) NULL,
 CONSTRAINT [PK__users__B9BE370FB719034F] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__users__AB6E6164B9D97C16] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSubscriptions]    Script Date: 9/3/2024 12:08:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSubscriptions](
	[UserSubscriptionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SubscriptionId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserSubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[albums] ADD  CONSTRAINT [DF__albums__created___44FF419A]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[artists] ADD  CONSTRAINT [DF__artists__created__45F365D3]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[listening_history] ADD  DEFAULT (getdate()) FOR [played_at]
GO
ALTER TABLE [dbo].[playlist_songs] ADD  DEFAULT (getdate()) FOR [added_at]
GO
ALTER TABLE [dbo].[playlists] ADD  CONSTRAINT [DF__playlists__creat__48CFD27E]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[songs] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[user_favorites] ADD  DEFAULT (getdate()) FOR [added_at]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__created_a__4BAC3F29]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF__users__role__656C112C]  DEFAULT ('user') FOR [role]
GO
ALTER TABLE [dbo].[album_songs]  WITH CHECK ADD FOREIGN KEY([album_id])
REFERENCES [dbo].[albums] ([album_id])
GO
ALTER TABLE [dbo].[album_songs]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[albums]  WITH CHECK ADD  CONSTRAINT [FK__albums__artist_i__4D94879B] FOREIGN KEY([artist_id])
REFERENCES [dbo].[artists] ([artist_id])
GO
ALTER TABLE [dbo].[albums] CHECK CONSTRAINT [FK__albums__artist_i__4D94879B]
GO
ALTER TABLE [dbo].[artist_songs]  WITH CHECK ADD FOREIGN KEY([artist_id])
REFERENCES [dbo].[artists] ([artist_id])
GO
ALTER TABLE [dbo].[artist_songs]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[artists]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[listening_history]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[listening_history]  WITH CHECK ADD  CONSTRAINT [FK__listening__user___4F7CD00D] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[listening_history] CHECK CONSTRAINT [FK__listening__user___4F7CD00D]
GO
ALTER TABLE [dbo].[otp_code]  WITH CHECK ADD  CONSTRAINT [FK__OTPCode__Created__59FA5E80] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[otp_code] CHECK CONSTRAINT [FK__OTPCode__Created__59FA5E80]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscriptions] ([SubscriptionId])
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[playlist_songs]  WITH CHECK ADD  CONSTRAINT [FK__playlist___playl__5070F446] FOREIGN KEY([playlist_id])
REFERENCES [dbo].[playlists] ([playlist_id])
GO
ALTER TABLE [dbo].[playlist_songs] CHECK CONSTRAINT [FK__playlist___playl__5070F446]
GO
ALTER TABLE [dbo].[playlist_songs]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[playlists]  WITH CHECK ADD  CONSTRAINT [FK__playlists__user___52593CB8] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[playlists] CHECK CONSTRAINT [FK__playlists__user___52593CB8]
GO
ALTER TABLE [dbo].[refresh_token]  WITH CHECK ADD  CONSTRAINT [FK_RefreshToken_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[refresh_token] CHECK CONSTRAINT [FK_RefreshToken_User]
GO
ALTER TABLE [dbo].[user_favorites]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[user_favorites]  WITH CHECK ADD  CONSTRAINT [FK__user_favo__user___5535A963] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[user_favorites] CHECK CONSTRAINT [FK__user_favo__user___5535A963]
GO
ALTER TABLE [dbo].[user_queues]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[user_queues]  WITH CHECK ADD FOREIGN KEY([song_id])
REFERENCES [dbo].[songs] ([song_id])
GO
ALTER TABLE [dbo].[UserSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_UserSubscriptions_Subscriptions] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscriptions] ([SubscriptionId])
GO
ALTER TABLE [dbo].[UserSubscriptions] CHECK CONSTRAINT [FK_UserSubscriptions_Subscriptions]
GO
ALTER TABLE [dbo].[UserSubscriptions]  WITH CHECK ADD  CONSTRAINT [FK_UserSubscriptions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[UserSubscriptions] CHECK CONSTRAINT [FK_UserSubscriptions_Users]
GO
USE [master]
GO
ALTER DATABASE [music_streaming] SET  READ_WRITE 
GO
