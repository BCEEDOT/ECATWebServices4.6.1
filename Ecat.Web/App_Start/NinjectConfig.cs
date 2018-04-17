using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Breeze.ContextProvider.EF6;
using Ecat.DesignerMod.Core.Business;
using Ecat.DesignerMod.Core.Data;
using Ecat.DesignerMod.Core.Interface;
using Ecat.FacMod.Core;
using Ecat.LmsAdmin.Mod;
using Ecat.Shared.Core;
using Ecat.Shared.Core.Interface;
using Ecat.Shared.DbMgr.Context;
using Ecat.StudMod.Core;
using Ecat.UserMod.Core;
using Ecat.Web.Provider;
using Ecat.Web.Utility;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc;
using Ninject.Web.WebApi.Filter;
using Ninject.Web.WebApi.FilterBindingSyntax;
using FilterScope = System.Web.Http.Filters.FilterScope;

namespace Ecat.Web
{
    public static class NinjectConfig
    {
        public static IKernel GetKernal()
        {
            var kernel = new StandardKernel();

            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));


            kernel.Load(Assembly.GetExecutingAssembly());
            //kernel.BindHttpFilter<AuthorizedFilterService>(FilterScope.Controller)
            //    .WhenControllerHas<EcatRolesAuthorized>()
            //    .WithConstructorArgumentFromControllerAttribute<EcatRolesAuthorized>("roles", attribute => attribute.Is);    

            kernel.Bind<ITests>().To<DependencyTest>();
            kernel.Bind<EcatContext>().ToSelf().InRequestScope();
            kernel.Bind<IUserLogic>().To<UserLogic>().InRequestScope();
            kernel.Bind<IDesignerLogic>().To<DesignerLogic>().InRequestScope();
            kernel.Bind<IFacLogic>().To<FacLogic>().InRequestScope();
            kernel.Bind<IFacRepo>().To<FacRepo>().InRequestScope();
            kernel.Bind<IStudLogic>().To<StudLogic>().InRequestScope();
            kernel.Bind<ILmsAdminCourseOps>().To<CourseOps>().InRequestScope();
            kernel.Bind<ILmsAdminGroupOps>().To<GroupOps>().InRequestScope();
            kernel.Bind<EFContextProvider<UserCtx>>().ToSelf().InRequestScope();
            kernel.Bind<EFContextProvider<StudCtx>>().ToSelf().InRequestScope();
            kernel.Bind<EFContextProvider<FacCtx>>().ToSelf().InRequestScope();
            kernel.Bind<EFContextProvider<DesignerCtx>>().ToSelf().InRequestScope();
            kernel.Bind<UserCtx>().ToSelf().InRequestScope();
            kernel.Bind<StudCtx>().ToSelf().InRequestScope();
            kernel.Bind<DesignerCtx>().ToSelf().InRequestScope();
            kernel.Bind<FacCtx>().ToSelf().InRequestScope();
            kernel.Bind<BbWsCnet>().ToSelf().InRequestScope();

            return kernel;
        }
    }
}