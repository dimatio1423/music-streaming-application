using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.OtpRepos
{
    public class OTPRepository : GenericRepository<OtpCode>, IOTPRepository
    {
        private readonly MusicStreamingContext _context;

        public OTPRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OtpCode>> GetLastestOTPList(int userId)
        {
            return await _context.OtpCodes.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
    }
}
