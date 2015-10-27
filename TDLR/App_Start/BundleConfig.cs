using System.Web.Optimization;

namespace Tdlr
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui.min.js",
                "~/Scripts/jquery.easing.min.js",
                "~/Scripts/jquery.validate.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui.min.js",
                "~/Scripts/jquery.easing.min.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/AadPickerLibrary").Include(
                    "~/Scripts/AadPickerLibrary.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/detect_aad").Include(
                    "~/Scripts/detect_aad.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/tasks").Include(
                    "~/Scripts/task-form.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/grayscale").Include(
                    "~/Scripts/grayscale.js"
                ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/homepage").Include(
                "~/Content/home.css"
            ));

            bundles.Add(new StyleBundle("~/Content/signin").Include(
                "~/Content/signin.css"
            ));

            bundles.Add(new StyleBundle("~/Content/tasks").Include(
                "~/Content/tasks.css"
            ));

            bundles.Add(new StyleBundle("~/Content/grayscale").Include(
                "~/Content/grayscale.css"
            ));

            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                "~/Content/font-awesome.min.css", 
                "~/Content/google_font_lora.css", 
                "~/Content/google_font_montserrat.css"
            ));
        }
    }
}