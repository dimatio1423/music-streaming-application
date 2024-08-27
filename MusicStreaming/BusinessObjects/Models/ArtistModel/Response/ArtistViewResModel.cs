using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistModel.Response
{
    public class ArtistViewResModel
    {
        public int ArtistId { get; set; }
        public string Name { get; set; } = null!;
        public string ImagePath { get; set; }
    }   
}
