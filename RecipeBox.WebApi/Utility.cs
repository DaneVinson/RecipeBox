using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// Static utility methods for RecipeBox.WebApi.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Extract data using the input MultipartFormDataStreamProvider and return an object of T
        /// represented in the data.
        /// source: http://www.mono-software.com/blog/post/Mono/233/Async-upload-using-angular-file-upload-directive-and-net-WebAPI-service/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        internal static T GetFormData<T>(MultipartFormDataStreamProvider provider)
        {
            if (provider.FormData.HasKeys())
            {
                string unescapedData = Uri.UnescapeDataString(provider.FormData.GetValues(0).FirstOrDefault());
                if (!String.IsNullOrWhiteSpace(unescapedData)) { return JsonConvert.DeserializeObject<T>(unescapedData); }
            }
            return default(T);
        }

        /// <summary>
        /// Get the temp directory path for the current HttpContext creating the directory if it doesn't exist.
        /// </summary>
        internal static string GetTempPath()
        {
            var path = HttpContext.Current.Server.MapPath(Utility.RelativeTempPath);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            return path;
        }


        /// <summary>
        /// The name of the account Id claim.
        /// </summary>
        public static readonly string AccountIdClaimName = "AccountId";

        /// <summary>
        /// The name of the client Id claim.
        /// </summary>
        public static readonly string ClientIdClaimName = "ClientId";

        private static readonly ILog Log = LogManager.GetLogger(typeof(Utility));

        /// <summary>
        /// Relative path to the temp directory.
        /// </summary>
        private static readonly string RelativeTempPath = "~/Temp";

        internal static readonly string SendGridUserName = ConfigurationManager.AppSettings["sendGridUserName"];

        internal static readonly string SendGridPassword = ConfigurationManager.AppSettings["sendGridPassword"];

        internal static readonly Size StandardImageSize = new Size(400, 400);

        internal static readonly string UserNameClaimName = "UserName";
    }
}