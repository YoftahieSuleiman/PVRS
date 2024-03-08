using System.Web;
using System.Web.Optimization;

namespace University
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.11.3.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));  

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker3.css",
                      "~/Content/site.css"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/modernizr-2.6.2.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/DatePickerReady.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
          "~/Scripts/jquery-ui-{version}.js"));
            //css  
            bundles.Add(new StyleBundle("~/Content/cssjqryUi").Include(
                   "~/Content/jquery-ui.css"));  
            

        }
    }
}
