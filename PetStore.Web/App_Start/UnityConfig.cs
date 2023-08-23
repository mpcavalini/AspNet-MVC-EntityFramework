using System.Web.Mvc;
using System.Web.Services;
using Unity;
using Unity.Mvc5;
using PetStore.Services.UnitOfWork;
using PetStore.Core.UnitOfWork;

namespace PetStore.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IPetUnitOfWork, PetUnitOfWork>(); // Example
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}