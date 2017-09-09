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
        /// <summary>
        /// Initialize the Unity Container
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            return container;
        }

        /// <summary>
        /// Builds the Unity Container
        /// </summary>
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        /// <summary>
        /// Register types within the unity container
        /// </summary>
        /// <param name="container">Unity container</param>
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
            container.RegisterType<IPriceRangeBus, PriceRangeBus>();
            container.RegisterType<IHorseAbilityBus, HorseAbilityBus>();
            //container.RegisterType<IUtilAdDao, UtilAdDao>();
            container.RegisterType<IUtilBus, UtilBus>();
            container.RegisterType<IRecommendedRiderDao, RecommendedRiderDao>();
            container.RegisterType<IRecommendedRiderBus, RecommendedRiderBus>();
            container.RegisterType<IAppointmentBus, AppointmentBus>();
            container.RegisterType<IAppointmentDao, AppointmentDao>();
            container.RegisterType<ICountryBus, CountryBus>();
            container.RegisterType<ICountryDao, CountryDao>();
            container.RegisterType<IRefreshTokenDao, RefreshTokenDao>();
            container.RegisterType<IClientDao, ClientDao>();

            container.RegisterInstance(typeof(HttpConfiguration), GlobalConfiguration.Configuration);
        }
    }
}