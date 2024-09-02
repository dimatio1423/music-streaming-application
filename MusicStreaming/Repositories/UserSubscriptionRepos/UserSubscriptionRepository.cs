using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.SubscriptionUserRepos
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        private readonly MusicStreamingContext _context;

        public UserSubscriptionRepository(MusicStreamingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserSubscription>> GetUserSubscriptionsBySubscriptionId(int subscriptionId)
        {
            return await _context.UserSubscriptions.Where(x => x.SubscriptionId == subscriptionId).ToListAsync();
        }

        public async Task<List<UserSubscription>> GetUserSubscriptionsOfUser(int userId)
        {
            return await _context.UserSubscriptions
                .Where(x => x.UserId == userId
                    && x.IsActive
                    && (x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now)).ToListAsync();
        }
    }
}
