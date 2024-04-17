namespace SBM_Captcha_ASP
{
    public interface ISessionProvider
    {
        void SetSession(string key, SBMCaptchaSession value);
        SBMCaptchaSession GetSession(string key);
    }
}
