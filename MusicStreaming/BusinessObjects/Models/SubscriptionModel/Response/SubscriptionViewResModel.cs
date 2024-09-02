using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SubscriptionModel.Response
{
    public class SubscriptionViewResModel
    {
        public int SubscriptionId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int DurationInDays { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}
