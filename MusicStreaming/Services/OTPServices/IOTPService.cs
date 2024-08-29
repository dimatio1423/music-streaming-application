using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OTPServices
{
    public interface IOTPService
    {
        public string GenerateOTP();

        Task<string> CreateOTPCodeForEmail(int userId);

        Task VerifyOTP(string Otp, int userId);
    }
}
