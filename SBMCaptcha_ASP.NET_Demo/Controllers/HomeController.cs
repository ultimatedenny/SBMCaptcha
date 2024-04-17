using System.Web.Mvc;
namespace SBMCaptcha_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Captcha");
        }
    }
}