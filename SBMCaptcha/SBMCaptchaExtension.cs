using System.IO;
using System.Net;
using System.Web;

namespace SBM_Captcha_ASP
{
    public static class SBMCaptchaExtension
    {
        public static SBMCaptchaResult CallSBMCaptcha(SBMCaptcha captcha, HttpRequest request, HttpResponse response)
        {
            bool isAjaxRequest = request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest && request.Form != null)
            {
                int captchaId = Utils.ConvertToInt(request["cID"]);
                int requestType = Utils.ConvertToInt(request["rT"]);

                if (captchaId > -1 && requestType > -1)
                {
                    switch (requestType)
                    {
                        case 1:
                            return new SBMCaptchaResult(SBMCaptchaState.CaptchaHashesReturned, captcha.GetCaptchaData(captchaId, request["tM"]));
                        case 2:
                            if (captcha.SetSelectedAnswer(captchaId, request["pC"]))
                            {
                                response.StatusCode = (int)HttpStatusCode.OK;
                                response.End();
                                return new SBMCaptchaResult(SBMCaptchaState.CaptchaIconSelected, null);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                int captchaId = Utils.ConvertToInt(request["cid"]);

                if (captchaId > -1)
                {
                    FileStream image = captcha.GetIconFromHash(captchaId, request["hash"]);
                    if (image != null)
                    {
                        return new SBMCaptchaResult(SBMCaptchaState.CaptchaImageReturned, image);
                    }
                }
            }

            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.End();

            return new SBMCaptchaResult(SBMCaptchaState.CaptchaGeneralFail, null);
        }
    }
}
