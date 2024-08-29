using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.SongRepos
{
    public interface ISongRepository : IGenericRepository<Song>
    {
        Task<int> AddSong(Song song);
        Task<List<Song>> GetSongs(int? page, int? size);

        Task<List<Song>> GetSongsByAlbum(int albumId, int? page, int? size);

        Task<List<Song>> GetSongsByArtist(int artistId, int? page, int? size);

        Task<List<Song>> GetSongsByArtist(int artistId);

        Task<List<Song>> GetUserFavoriteSongs(List<int> songIds, int? page, int? size);

        Task<List<Song>> GetUserListeningHisotry(List<int> songIds, int? page, int? size);
        Task<List<Song>> GetRecommendSongsForUser(int userId, int? page, int? size);

        Task<List<Song>> SearchBySongName(string songName, int? page, int? size);
     }
}
