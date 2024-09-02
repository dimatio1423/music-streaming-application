using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.UserModels.Request
{
    public class UserViewReqModel
    {
        public List<string>? subscription { get; set; }
        public List<string>? role { get; set; }
        public List<string>? status { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? sortBy { get; set; }
    }
}
