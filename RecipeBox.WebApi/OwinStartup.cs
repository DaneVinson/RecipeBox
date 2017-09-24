using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Managers;
using RecipeBox.DataContext;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(RecipeBox.WebApi.Startup))]
namespace RecipeBox.WebApi
{
    public class Startup
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Startup));


        // Runs by name convention.
        public void Configuration(IAppBuilder app)
        {
            // Cross-cutting concerns
            log4net.Config.XmlConfigurator.Configure();
            ConfigureOAuth(app);

            // Setup CORS
            app.UseCors(CorsOptions.AllowAll);

            // Basic configuration
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            // Ninject the app
            app.UseNinjectMiddleware(CreateKernel)
                .UseNinjectWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(7),
                Provider = CreateKernel().Get<IOAuthAuthorizationServerProvider>(),  // TODO: This sucks but at least its DI
                RefreshTokenProvider = null // TODO: implement IAuthenticationTokenProvider or refresh tokens, ref http://stackoverflow.com/questions/20637674/owin-security-how-to-implement-oauth2-refresh-tokens
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        private StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Bind<IAuthContext>().To<AuthContext>();
            kernel.Bind<IRecipeBoxContext>().To<RecipeBoxContext>();
            kernel.Bind<IOAuthAuthorizationServerProvider>().To<AuthServiceProvider>();
            kernel.Bind<IClaimsProvider>().To<ClaimsProvider>();
            kernel.Bind<IDataManager<Account>>().To<AccountDataManager>();
            kernel.Bind<IDataManager<Tag>>().To<TagDataManager>();
            kernel.Bind<IDataManager<Recipe>>().To<RecipeDataManager>();

// TODO: I hate using DEBUG pre-processor. Revisit with DI later.
#if DEBUG
            kernel.Bind<IBlobStorage>().To<WebServerFileStore>();
#else
            kernel.Bind<IBlobStorage>().To<AzureBlobStore>();
#endif
            return kernel;
        }
    }
}