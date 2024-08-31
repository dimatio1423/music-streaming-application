using BusinessObjects.Entities;
using Repositories.OtpRepos;
using Repositories.UserRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.OTPServices
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _oTPRepository;
        private readonly IUserRepository _userRepository;

        public OTPService(IOTPRepository oTPRepository, IUserRepository userRepository)
        {
            _oTPRepository = oTPRepository;
            _userRepository = userRepository;
        }
        public async Task<string> CreateOTPCodeForEmail(int userId)
        {
            var currUser = await _userRepository.Get(userId);

            if (currUser == null)
            {
                throw new Exception("User does not exist");
            }

            var LatestOTPList = await _oTPRepository.GetLastestOTPList(currUser.UserId);
            var latestOTP = LatestOTPList.FirstOrDefault();

            if (latestOTP != null)
            {
                if ((DateTime.UtcNow - latestOTP.CreatedAt).TotalMinutes < 2)
                {
                    throw new Exception($"Cannot send new OTP right now, please wait for {(120 - (DateTime.UtcNow - latestOTP.CreatedAt).TotalSeconds).ToString("0.00")} second(s)");
                }
            }

            string newOTP = GenerateOTP();

            OtpCode otpCode = new OtpCode
            {
                OptCode = newOTP,
                CreatedAt = DateTime.UtcNow,
                UserId = currUser.UserId,
                IsUsed = false,
            };


            await _oTPRepository.Insert(otpCode);

            return otpCode.OptCode;
        }

        public string GenerateOTP()
        {
            var otp = new Random().Next(100000, 999999).ToString();

            return otp;
        }

        public async Task VerifyOTP(string Otp, int userId)
        {
            var currUser = await _userRepository.Get(userId);

            if (currUser == null)
            {
                throw new Exception("User does not exist");
            }

            var LatestOTPList = await _oTPRepository.GetLastestOTPList(currUser.UserId);
            var latestOTP = LatestOTPList.FirstOrDefault();

            if (latestOTP != null)
            {
                if ((DateTime.UtcNow - latestOTP.CreatedAt).TotalMinutes > 30 || latestOTP.IsUsed)
                {
                    throw new Exception("The OTP is already used or expired");
                }

                if (!Otp.Equals(latestOTP.OptCode))
                {
                    throw new Exception("The OTP is incorrect");
                }else
                {
                    latestOTP.IsUsed = true;
                }

                await _oTPRepository.Update(latestOTP);

            }
        }
    }
}
