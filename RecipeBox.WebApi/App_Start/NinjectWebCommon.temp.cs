using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using RecipeBox.Core.Interfaces;
using RecipeBox.DataContext;
using System;
using System.Web;
using System.Web.Http;

public static class NinjectWebCommon 
{
    private static readonly Bootstrapper bootstrapper = new Bootstrapper();

    /// <summary>
    /// Creates the kernel that will manage your application.
    /// </summary>
    /// <returns>The created kernel.</returns>
    private static IKernel CreateKernel()
    {
        var kernel = new StandardKernel();

        kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
        kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
        kernel.Bind<IRecipeBoxContext>().To<RecipeBoxContext>();

        return kernel;
    }
        
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
}
