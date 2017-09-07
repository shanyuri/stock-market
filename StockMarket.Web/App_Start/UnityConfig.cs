using Microsoft.Practices.Unity;
using StockMarket.Controllers;
using StockMarket.Infrastructure.Context;
using StockMarket.Infrastructure.Services;
using StockMarket.Jobs;
using System;

namespace StockMarket.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());
            container.RegisterType<IStockMarketDbContext, StockMarketDbContext>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IStockService, StockService>();
            container.RegisterType<IStockUpdateTimer, StockUpdateTimer>(new ContainerControlledLifetimeManager());
        }
    }
}
