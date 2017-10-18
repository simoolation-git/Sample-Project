[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Wedding.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Wedding.App_Start.NinjectWebCommon), "Stop")]

namespace Wedding.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Services;
    using Domain.Services.Interfaces;
    using Search;
    using global::Search;
    using Domain.Interfaces.Repositories;
    using global::Repository.Implementation;
    using global::Services.Implementation;
    using Domain.Interfaces.Services;
    using Domain.Services;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
            kernel.Bind<ICacheService>().To<RedisCacheService>().InSingletonScope();
            kernel.Bind<IAzureSearchService>().To<AzureSearchService>().InSingletonScope();
            kernel.Bind<ISearchTermRepository>().To<SearchTermRepository>().InSingletonScope();
            kernel.Bind<IPhotoService>().To<PhotoService>().InSingletonScope();
            kernel.Bind<IOpinionsTableStorage>().To<OpinionsTableStorage>().InSingletonScope();
            kernel.Bind<ICommentTableStorage>().To<CommentTableStorage>().InSingletonScope();
            kernel.Bind<IPostedItemService>().To<PostedItemService>();
            kernel.Bind<ISocialService>().To<SocialService>();
            kernel.Bind<ISitemapService>().To<SitemapService>();            
        }
    }
}