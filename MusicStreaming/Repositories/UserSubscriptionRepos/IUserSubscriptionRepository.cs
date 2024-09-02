using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.SubscriptionUserRepos
{
    public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
    {
        Task<List<UserSubscription>> GetUserSubscriptionsBySubscriptionId(int subscriptionId);
        Task<List<UserSubscription>> GetUserSubscriptionsOfUser(int userId);
    }
}
