using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ArtistRepos
{
    public class ArtistRepository : GenericRepository<Artist>, IArtistRepository
    {
        private readonly MusicStreamingContext _context;

        public ArtistRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Artist> GetArtist(int artistId)
        {
            try
            {
                return await _context.Artists
                    .Include(x => x.User)
                    .Include(x => x.Albums)
                    .Where(x => x.ArtistId == artistId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Artist> GetArtistByUserId(int userId)
        {
            try
            {
                return await _context.Artists.Include(x => x.User).Where(x => x.UserId == userId).FirstOrDefaultAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Artist>> GetArtists(int? page, int? size)
        {
            try
            {
                var pageIndex = (page.HasValue && page > 0) ? page.Value : 1;
                var sizeIndex = (size.HasValue && size > 0) ? size.Value : 10;

                return await _context.Artists.Include(x => x.User)
                                     .Include(x => x.Albums)
                    .Skip((pageIndex - 1) * sizeIndex)
                    .Take(sizeIndex)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
