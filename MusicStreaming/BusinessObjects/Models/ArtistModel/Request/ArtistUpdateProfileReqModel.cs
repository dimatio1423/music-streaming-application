using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistModel.Request
{
    public class ArtistUpdateProfileReqModel
    {
        public string Name { get; set; } = null!;

        public string? Bio { get; set; }

        public IFormFile? ImagePath { get; set; }
    }
}
