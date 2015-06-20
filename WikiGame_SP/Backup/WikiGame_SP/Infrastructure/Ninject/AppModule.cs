using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Ninject.Activation;
using WikiGame.Models;
using System.Web.Mvc;

namespace WikiGame.Infrastructure.Ninject
{
    public class AppModule : NinjectModule
    {

        public override void Load()
        {
            Bind<ICategoryProvider>().To<TestCategoryProvider>().InSingletonScope();
        }

        private object getCurrentClassName(IContext r)
        {
            if (r.Request.ParentContext == null)
                return "WikiGameApp";
            else
                return r.Request.ParentContext.Request.Service.FullName;
        }
    }
}