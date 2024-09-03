﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.ArtistModel.Request
{
    public class ArtistSearchReqModel
    {
        public string? searchValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? sortBy { get; set; }
    }
}
