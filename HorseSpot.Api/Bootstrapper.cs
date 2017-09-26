using HorseSpot.BLL.Bus;
using HorseSpot.BLL.Interfaces;
using HorseSpot.DAL.Dao;
using HorseSpot.DAL.Interfaces;
using HorseSpot.Infrastructure.MailService;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using System.Web.Http;
using System.Web.Mvc;

namespace HorseSpot.Api
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

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
            container.RegisterType<IPriceRangeDao, PriceRangeDao>();
            container.RegisterType<IHorseAbilityDao, HorseAbilityDao>();
            container.RegisterType<IUtilBus, UtilBus>();
            container.RegisterType<IRecommendedRiderDao, RecommendedRiderDao>();
            container.RegisterType<IAppointmentBus, AppointmentBus>();
            container.RegisterType<IAppointmentDao, AppointmentDao>();
            container.RegisterType<ICountryDao, CountryDao>();
            container.RegisterType<IRefreshTokenDao, RefreshTokenDao>();
            container.RegisterType<IClientDao, ClientDao>();
            container.RegisterType<IImageDao, ImageDao>();

            container.RegisterInstance(typeof(HttpConfiguration), GlobalConfiguration.Configuration);
        }
    }
}