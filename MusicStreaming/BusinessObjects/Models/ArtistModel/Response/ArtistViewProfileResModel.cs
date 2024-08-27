using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistModel.Response
{
    public class ArtistViewProfileResModel
    {
        public int ArtistId { get; set; }

        public string ArtistName { get; set; } = null!;

        public string? Bio { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? ImagePath { get; set; }

        public string Role { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}
