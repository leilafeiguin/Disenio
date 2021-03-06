﻿using Diseño.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Diseño
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DondeInviertoInitializer initializer = new DondeInviertoInitializer();
            Database.SetInitializer(initializer);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
