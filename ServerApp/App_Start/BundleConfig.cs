using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace ServerApp.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Script/Bundles").Include(
                                       "~/Scripts/Projects/ClientApp/runtime-*",
                                        "~/Scripts/Projects/ClientApp/polyfills-*",
                                       "~/Scripts/Projects/ClientApp/main-*"));
#if (!DEBUG)
            BundleTable.EnableOptimizations = true;
#endif

            //bundles.Add(new StyleBundle("~/Content/Styles").Include("~/bundles/styles.*"));
        }

    }
}