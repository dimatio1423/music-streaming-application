using BusinessObjects.Entities;
using Repositories.GenericRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.OtpRepos
{
    public interface IOTPRepository : IGenericRepository<OtpCode>
    {
        Task<List<OtpCode>> GetLastestOTPList(int userId);
    }
}
