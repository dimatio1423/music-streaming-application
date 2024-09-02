using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.PlaylistSongRepos
{
    public interface IPlaylistSongRepository : IGenericRepository<PlaylistSong>
    {
        Task<List<PlaylistSong>> GetPlaylistSongsByPlaylistId(int playlistId);
        Task<List<PlaylistSong>> GetPlaylistSongsBySongId(int songId);
        Task<List<PlaylistSong>> GetPlaylistSongsBySongIds(List<int> songIds);
        Task<PlaylistSong> CheckSongExistingInPlaylist(int songId, int playlistId);

    }
}
