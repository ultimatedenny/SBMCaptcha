using System;
namespace SBM_Captcha_ASP
{
    public class SBMCaptchaResult
    {
        public SBMCaptchaState CaptchaState { get; set; }
        public Object CaptchaResult { get; set; }

        public SBMCaptchaResult(SBMCaptchaState state, Object result)
        {
            CaptchaState = state;
            CaptchaResult = result;
        }
    }
}