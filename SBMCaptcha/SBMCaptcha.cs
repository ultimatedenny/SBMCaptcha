using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace SBM_Captcha_ASP
{
    public class SBMCaptcha
    {
        private readonly ISessionProvider _sessionProvider;
        private SBMCaptchaSession _sessionData;
        private SBMCaptchaSession SessionHolder
        {
            get { return _sessionProvider.GetSession("SBM_Captcha_asp"); }
            set { _sessionProvider.SetSession("SBM_Captcha_asp", value); }
        }
        private List<string> errorMessages;
        private readonly string _iconPath;
        public SBMCaptcha(ISessionProvider sessionProvider, string path, string[] messages = null)
        {
            _sessionProvider = sessionProvider;
            _iconPath = path;
            SetErrorMessages(messages);
        }
        public void SetErrorMessages(string[] messages)
        {
            if (messages != null && messages.Length == 4)
            {
                errorMessages = messages.ToList();
            }
            else
            {
                errorMessages = new string[4].ToList();
            }
        }
        public string[] GetCaptchaData(int captchaId, string theme)
        {
            int a = Utils.Random.Next(1, 91);
            int b = 0;
            _sessionData = SessionHolder;
            if(_sessionData == null)
            {
                theme = (string.IsNullOrEmpty(theme)) ? Utils.SanitizeString(theme) : "light";
                _sessionData = new SBMCaptchaSession(theme);
            }
            int c = -1;
            while (b == 0) {
                c = Utils.Random.Next(1, 91);
                if (c != a) b = c;
            }
            int d = -1;
            List<string> e = new List<string>();
            while (d == -1) {
                int f = Utils.Random.Next(1, 5);
                int g = (_sessionData.LastClicked > -1) ? _sessionData.LastClicked : 0;

                if (f != g) d = f;
            }
            for (int i = 1; i < 6; i++) {
                if (i == d) {
                    e.Add(GetImageHash("icon-" + a + "-" + i, captchaId));
                } else {
                    e.Add(GetImageHash("icon-" + b + "-" + i, captchaId));
                }
            }
            _sessionData.Clear();
            _sessionData.CorrectId = a;
            _sessionData.IncorrectId = b;
            _sessionData.Hashes = e;
            _sessionData.CorrectHash = e[d - 1];
            _sessionData.IconRequests = 0;
            SessionHolder = _sessionData;
            return e.ToArray();
        }
        public bool SetSelectedAnswer(int captchaId, string pickedHash)
        {
            if(captchaId < 0)
            {
                return false;
            }
            if (_sessionData == null)
            {
                _sessionData = SessionHolder;
            }
            if (!string.IsNullOrEmpty(pickedHash) && (GetCorrectIconHash(captchaId) == pickedHash)) {
                _sessionData.Completed = true;
                _sessionData.Clear();
                SessionHolder = _sessionData;
                return true;
            } else {
                _sessionData.Completed = false;
                SessionHolder = _sessionData;
                if (_sessionData.Hashes.Contains(pickedHash))
                {
                    int index = _sessionData.Hashes.IndexOf(pickedHash);
                    _sessionData.LastClicked = index + 1;
                }
            }
            return false;
        }
        public bool ValidateSubmission(HttpRequest request)
        {
            if (request.Form != null)
            {
                int captchaId = Utils.ConvertToInt(request["captcha-idhf"]);
                if (captchaId < 0)
                {
                    throw new SBMCaptchaException(GetErrorMessage(3, "The captcha ID was invalid."));
                }
                if (_sessionData == null)
                {
                    _sessionData = SessionHolder;
                }
                if (!string.IsNullOrEmpty(request["captcha-hf"]))
                {
                    string hash = GetCorrectIconHash(captchaId);
                    if (_sessionData.Completed == true && hash == request["captcha-hf"])
                    {
                        return true;
                    }
                    else
                    {
                        throw new SBMCaptchaException(GetErrorMessage(0, "You've selected the wrong image."));
                    }
                }
                else
                {
                    throw new SBMCaptchaException(GetErrorMessage(1, "No image has been selected."));
                }
            }
            else
            {
                throw new SBMCaptchaException(GetErrorMessage(2, "You've not submitted any form."));
            }
        }
        public FileStream GetIconFromHash(int captchaId, string hash)
        {
            if ((!string.IsNullOrEmpty(hash) && hash.Length == 48) && captchaId > -1) {
                _sessionData = SessionHolder;
                if (_sessionData.IconRequests >= 5) {
                    throw new SBMCaptchaException("You are not allowed to view this page.");
                }
                _sessionData.IconRequests += 1;
                SessionHolder = _sessionData;
                if (_sessionData.Hashes.Contains(hash))
                {
                    var imagePath = GetIconFilePath(captchaId, hash);
                    FileStream iconFile = GetFileStream(imagePath);
                    if (iconFile != null) {
                        return iconFile;
                    }
                }
            }

            return null;
        }
        private string GetCorrectIconHash(int captchaId) 
        {
            if (_sessionData == null)
                return string.Empty;
            return (captchaId > -1) ? _sessionData.CorrectHash : string.Empty;
        }
        private string GetImageHash(string image, int captchaId) 
        {
            string hash = (!string.IsNullOrEmpty(image) && captchaId > -1) 
                ? Utils.GetStringSha256Hash(image + Utils.Random.Next(1, 999999) + image.GetHashCode()) : string.Empty;
            return hash.Substring(0, 48);
        }
        private string GetErrorMessage(int message, string fallback)
        {
            if(!string.IsNullOrEmpty(errorMessages[message]) && message <= 4)
            {
                return errorMessages[message];
            }
            return fallback;
        }
        private string GetIconFilePath(int captchaId, string hash)
        {
            var imagePath = Path.Combine(_iconPath, _sessionData.Theme);
            imagePath = Path.Combine(imagePath, "icon-" + ((GetCorrectIconHash(captchaId) == hash) ? _sessionData.CorrectId : _sessionData.IncorrectId) + ".png");
            return imagePath;
        }
        private FileStream GetFileStream(string iconFilePath)
        {
            var file = new FileInfo(iconFilePath);
            if (file.Exists == false)
            {
                return null;
            }
            return new FileStream(file.FullName, FileMode.Open);
        }
    }
}
