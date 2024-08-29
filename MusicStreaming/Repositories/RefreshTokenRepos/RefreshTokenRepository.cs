using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.RefreshTokenRepos
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly MusicStreamingContext _context;

        public RefreshTokenRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            return await _context.RefreshTokens.Include(x => x.User).Where(x => x.RefreshToken1.Equals(refreshToken)).FirstOrDefaultAsync();
        }
    }
}
