using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.AlbumRepos
{
    public interface IAlbumRepository : IGenericRepository<Album>
    {
        Task<List<Album>> GetAlbums(int? page, int? size);

        Task<List<Album>> GetAlbumsByArtist(int artistId, int? page, int? size);

        Task<List<Album>> GetAlbumsByArtist(int artistId);

        Task<List<Album>> GetAlbumsByGenre(string genre, int? page, int? size);

        Task<List<Album>> GetRecommendAlbumsForUser(int userId, int? page, int? size);

        Task<List<Album>> GetAlbumsByAlbumIds(List<int> albumIds);
        Task<Album> GetAlbumsByAlbumId(int albumId);
    }
}
