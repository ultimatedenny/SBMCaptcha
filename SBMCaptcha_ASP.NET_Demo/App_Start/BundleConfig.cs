using System.Web;
using System.Web.Optimization;

namespace SBMCaptcha_ASP.NET
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sbm-captcha").Include(
                      "~/Scripts/sbm-captcha.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/sbm-captcha.css",
                      "~/Content/demo.css"));
        }
    }
}
