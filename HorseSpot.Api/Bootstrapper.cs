using System.Web.Http;
using System.Web.Mvc;
using HorseSpot.BLL.Bus;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Dao;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.MailService;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace HorseSpot.Api
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(UnityContainer container)
        {
            container.RegisterType<IUserBus, UserBus>();
            container.RegisterType<IUserDao, UserDao>();
            container.RegisterType<IHorseAdBus, HorseAdBus>();
            container.RegisterType<IHorseAdDao, HorseAdDao>();
            container.RegisterType<IAuthorizationBus, AuthorizationBus>();
            container.RegisterType<IMailerService, MailerService>();
            container.RegisterType<IUtilBus, UtilBus>();
            container.RegisterType<IAppointmentBus, AppointmentBus>();
            container.RegisterType<IAppointmentDao, AppointmentDao>();
            container.RegisterType<IRefreshTokenDao, RefreshTokenDao>();
            container.RegisterType<IClientDao, ClientDao>();
            container.RegisterType<IImageDao, ImageDao>();
            container.RegisterType<IUtilDao, UtilDao>();

            container.RegisterInstance(typeof(HttpConfiguration), GlobalConfiguration.Configuration);
        }
    }
}