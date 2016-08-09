using System;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;

using Serilog;
using SerilogWeb.Classic.Enrichers;

namespace GuildfordBoroughCouncil.Address.Api
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.LowercaseUrls = true;
            
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            var Logger = new LoggerConfiguration()
                .WriteTo.Seq(Properties.Settings.Default.SeqLogUrl, apiKey: Properties.Settings.Default.SeqLogKey, bufferBaseFilename: AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Logs")
                .Enrich.WithMachineName()
                .Enrich.With<HttpRequestUrlEnricher>()
                .Enrich.WithThreadId()
                .Enrich.With<HttpRequestClientHostIPEnricher>()
                .Enrich.WithProcessId()
                .Enrich.With<HttpRequestIdEnricher>()
                .Enrich.With<HttpRequestTypeEnricher>()
                .Enrich.With<HttpRequestUserAgentEnricher>()
                .Enrich.With<HttpRequestUrlReferrerEnricher>();

            Log.Logger = Logger.CreateLogger();

            Log.Debug("Address API started.");
        }
    }
}