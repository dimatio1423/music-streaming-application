using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.SubscriptionModel.Request
{
    public class SubscriptionAddReqModel
    {

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(49, ErrorMessage = "Name must be less than 50 characters.")]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day.")]
        public int DurationInDays { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }
    }
}
