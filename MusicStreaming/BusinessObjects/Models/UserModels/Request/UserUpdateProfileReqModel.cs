using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models.UserModels.Request
{
    public class UserUpdateProfileReqModel
    {

        [MaxLength(25, ErrorMessage = "Username contains 25 maximum 25 characters")]
        public string Username { get; set; }
        public IFormFile ImagePath { get; set; }
    }
}
