using BusinessObjects.Entities;
using BusinessObjects.Models.ResultModels;
using BusinessObjects.Models.SongModels.Request;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SongServices
{
    public interface ISongService
    {
        Task<ResultModel> GetSong(int songId);

        Task<ResultModel> GetSongs(int? page, int? size);

        Task<ResultModel> GetSongsByAlbum(int albumId, int? page, int? size);

        Task<ResultModel> GetSongsByArtist(int artistId, int? page, int? size);

        Task<ResultModel> GetUserFavoriteSongs(int? page, int? size, string token);

        Task<ResultModel> GetUserListeningHisotry(int? page, int? size, string token);

        Task<ResultModel> GetUserQueueSong(int? page, int? size, string token);

        Task<ResultModel> GetRecommendSongsForUser(int? page, int? size, string token);

        Task<ResultModel> AddNewSong(SongCreateReqModel songCreateReq, string token);

        Task<ResultModel> AddSongToPlaylist(int songId, int playlistId, string token);

        Task<ResultModel> RemoveSongFromPlaylist(int songId, int playlistId, string token);

        Task<ResultModel> CheckSongExistingInPlaylist(int songId, int playlistId);

        public Task<ResultModel> AddSongToAlbums(int songId, int albumId, string token);

        public Task<ResultModel> RemoveSongFromAlbum(int songId, int albumId, string token);

        Task<ResultModel> CheckSongExistingInAlbum(int songId, int albumId);

        Task<ResultModel> AddSongToUserFavorite(int songId, string token);

        Task<ResultModel> RemoveSongFromUserFavorite(int songId, string token);

        Task<ResultModel> CheckSongInUserFavorite(int songId, string token);

        Task<ResultModel> UpdateSong(SongUpdateReqModel songCreateReq, string token);

        Task<ResultModel> RemoveSong(int songId, string token);

        Task<ResultModel> Search(string searchValue, string filter, int? page, int? size);

        Task<ResultModel> PlaySong(int songId, string token);

        Task<ResultModel> PauseSong(PauseSongReqModel pauseSongReqModel, string token);

        Task<ResultModel> ReplaySong(int songId, string token);

        Task<ResultModel> AddSongToUserQueue(int songId, string token);

        Task<ResultModel> RemoveSongFromUserQueue(int songId, string token);

    }
}
