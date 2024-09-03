using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UserQueueRepos
{
    public interface IUserQueueRepository : IGenericRepository<UserQueue> 
    {
        Task<List<UserQueue>> GetUserQueue(int userId, int? page, int? size);
        Task<List<UserQueue>> GetUserQueueBySongIds(List<int> songIds);
        Task<List<UserQueue>> GetUserQueue(int userId);
        Task<UserQueue> CheckSongInUserQueue(int userId, int songId);

    }
}
