using BusinessObjects.Models.UserModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.PlaylistModels.Response
{
    public class PlaylistViewResModel
    {
        public int PlaylistId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public UserViewResModel User { get; set; }
    }
}
