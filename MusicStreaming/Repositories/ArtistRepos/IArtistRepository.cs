using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ArtistRepos
{
    public interface IArtistRepository : IGenericRepository<Artist>
    {
        Task<Artist> GetArtistByUserId(int userId);
        Task<Artist> GetArtist(int artistId);
        Task<List<Artist>> GetArtists(int? page, int?size);
        Task<List<Artist>> SearchByArtistName(string artistName, int? page, int?size);
    }
}
