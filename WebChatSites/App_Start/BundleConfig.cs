﻿using System.Web;
using System.Web.Optimization;

namespace WebChatSites
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/Chart").Include("~/Content/moment.min.js", "~/Content/Chart.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/lazyload").Include("~/Scripts/jquery.lazyload.js", "~/Scripts/jquery.lazyload.min.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/css/bootstrap.css", "~/Content/css/bootstrap-theme.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/WeChat.css"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/contentjs").Include("~/Content/WeChat.js"));
            

        }
    }
}
