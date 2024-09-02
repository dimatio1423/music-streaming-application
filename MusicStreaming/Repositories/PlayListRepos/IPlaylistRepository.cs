using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.PlayListRepos
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<List<Playlist>> GetPlaylistsByUserId(int userId, int? page, int? size);
        Task<List<Playlist>> GetPlaylistsByUserId(int userId);
        Task<List<Playlist>> SearchPlaylistByName(string playlistName, int? page, int? size);
    }
}
