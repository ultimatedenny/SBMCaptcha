using System;
using System.Collections.Generic;
namespace SBM_Captcha_ASP
{
    public class SBMCaptchaSession
    {
        public List<string> Hashes { get; set; }
        public int IconRequests { get; set; }
        public string Theme { get; set; }
        public int LastClicked { get; set; }
        public int CorrectId { get; set; }
        public int IncorrectId { get; set; }
        public string CorrectHash { get; set; }
        public bool Completed { get; set; }
        public SBMCaptchaSession(string theme = "light")
        {
            Theme = theme;
            Hashes = new List<string>();
            IconRequests = 0;
            LastClicked = -1;
            CorrectId = 0;
            IncorrectId = 0;
            CorrectHash = string.Empty;
            Completed = false;
        }
        public void Clear()
        {
            CorrectId = 0;
            IncorrectId = 0;
            Hashes = new List<string>();
            IconRequests = -1;
            LastClicked = 0;
        }
    }
}
