using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ArtistSongRepos
{
    public interface IArtistSongRepository : IGenericRepository<ArtistSong>
    {
        Task<ArtistSong> CheckArtistSongExisting(int artistId, int songId);
        Task<List<ArtistSong>> GetArtistSongBySongId(int songId);
    }
}
