using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.UserModels.Response
{
    public class UserLoginResModel
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? SubscriptionType { get; set; }

        public string? Role { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
