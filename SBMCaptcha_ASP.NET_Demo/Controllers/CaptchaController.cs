using SBM_Captcha_ASP;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SBMCaptcha_ASP.NET.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly SBMCaptcha _captcha;
        private readonly string _contentFolder = "Content/icons";
        HttpRequest request = System.Web.HttpContext.Current.Request;
        HttpResponse response = System.Web.HttpContext.Current.Response;
        public CaptchaController()
        {
            ISessionProvider sessionProvider = new HttpContextSession();
            string pathToContent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contentFolder);
            _captcha = new SBMCaptcha(sessionProvider, pathToContent);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public object GetCaptcha()
        {
            SBMCaptchaResult result = SBMCaptchaExtension.CallSBMCaptcha(_captcha, request, response);
            switch (result.CaptchaState)
            {
                case SBMCaptchaState.CaptchaHashesReturned:
                    return Json(result.CaptchaResult as string[], JsonRequestBehavior.AllowGet);
                case SBMCaptchaState.CaptchaImageReturned:
                    return File(result.CaptchaResult as FileStream, "image/png");
                case SBMCaptchaState.CaptchaIconSelected:
                case SBMCaptchaState.CaptchaGeneralFail:
                default:
                    return null;
            }
        }

        [HttpPost]
        public ActionResult SubmitForm()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            try
            {
                _captcha.ValidateSubmission(request);
                return View("Success");
            }
            catch(SBMCaptchaException ex)
            {
                return View(ex.Message);
            }
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Success()
        {
            return View();
        }
    }
    public class HttpContextSession : ISessionProvider
    {
        public SBMCaptchaSession GetSession(string key)
        {
            return (SBMCaptchaSession)HttpContext.Current.Session[key];
        }
        public void SetSession(string key, SBMCaptchaSession value)
        {
            HttpContext.Current.Session[key] = value;
        }
    }
}