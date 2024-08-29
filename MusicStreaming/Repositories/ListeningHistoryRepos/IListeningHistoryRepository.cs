using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.ListeningHistoryRepos
{
    public interface IListeningHistoryRepository : IGenericRepository<ListeningHistory>
    {
        public Task<List<ListeningHistory>> GetListeningHistoriesByUser(int userId, int? page, int? size);
        public Task<List<ListeningHistory>> GetListeningHistoriesBySongId(int songId);
        public Task<ListeningHistory> GetListeningHistoryByUserIdAndSongId(int userId, int songId);
    }
}
