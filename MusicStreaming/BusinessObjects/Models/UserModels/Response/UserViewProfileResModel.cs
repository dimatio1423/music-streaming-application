using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.UserModels.Response
{
    public class UserViewProfileResModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SubscriptionType { get; set; }

        public string Role { get; set; }

        public string ImagePath { get; set; }
    }
}
