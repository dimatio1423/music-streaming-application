using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SubscriptionModel.Request
{
    public class SubscriptionUpdateReqModel
    {
        [Required(ErrorMessage ="Id is required")]
        public int SubscriptionID { get; set; }

        public string? Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day.")]
        public int? DurationInDays { get; set; }

        public string? Description { get; set; }
    }
}
