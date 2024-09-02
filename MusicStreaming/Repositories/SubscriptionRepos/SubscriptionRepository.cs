using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.SubscriptionRepos
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        private readonly MusicStreamingContext _context;

        public SubscriptionRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Subscription> GetSubscriptionByName(string name)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(x => x.Name.ToLower().Contains(name.ToLower()));
        }
    }
}
