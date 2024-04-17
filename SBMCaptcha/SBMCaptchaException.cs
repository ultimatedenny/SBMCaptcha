using System;
namespace SBM_Captcha_ASP
{
    public class SBMCaptchaException : Exception
    {
        public SBMCaptchaException()
        {

        }
        public SBMCaptchaException(string message)
            : base(message)
        {

        }
        public SBMCaptchaException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}