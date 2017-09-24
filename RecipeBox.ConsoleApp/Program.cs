using RecipeBox.ConsoleApp.Migrations;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.DataContext;
using RecipeBox.Model.Attributes;
using RecipeBox.Model.Models;
using RecipeBox.WebApi;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace RecipeBox.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Migrate();
                //SendMail();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
        }


        private static void SendMail()
        {
            string emailAddress = "dane.vinson@gmail.com";
            string id = "xyxpdq";

            Uri baseUri = new Uri("https://recipeebox.azurewebsites.net");
            Uri confirmationUri = new Uri(String.Format("{0}/api/account/activate/{1}", baseUri.ToString(), id));
            var htmlStringBuilder = new StringBuilder().AppendFormat("To activate your <strong>RecipeBox</strong> account go <a href='{0}'>here</a>.", confirmationUri.ToString())
                                                        .Append("<br/>")
                                                        .Append("<br/>")
                                                        .AppendFormat("Thank you for trying <a href='{0}'>RecipeBox</a>. I hope you like it.", baseUri.ToString());
            var textStringBuilder = new StringBuilder().AppendFormat("To activate your RecipeBox account go to this address {0}", confirmationUri.ToString())
                                                        .AppendLine()
                                                        .AppendLine()
                                                        .AppendLine("Thank you for trying RecipeBox. I hope you like it.")
                                                        .AppendLine()
                                                        .AppendLine(baseUri.ToString());

            // Create the email object first, then add the properties.
            SendGridMessage myMessage = new SendGridMessage(
                                                new MailAddress("noreply@recipeebox.azurewebsites.net", "RecipeBox"),
                                                new MailAddress[] { new MailAddress(emailAddress) },
                                                "RecipeBox account registration email confirmation",
                                                htmlStringBuilder.ToString(),
                                                textStringBuilder.ToString());

            // Create an Web transport using credentials then use it to send the email.
            var transportWeb = new Web(new NetworkCredential("azure_87890e676ee903980504b776b04bd783@azure.com", "53sZNYxv5IDXEPT"));
            transportWeb.Deliver(myMessage);
        }

        private static void Migrate()
        {
            DbMigrator migrator = new DbMigrator(new ConfigurationWithSeed());
            var migrations = migrator.GetPendingMigrations();
            if (migrations.Count() == 0) { Console.WriteLine("No pending migrations."); }
            foreach (var migration in migrations)
            {
                Console.WriteLine("Applying migration {0}.", migration);
                migrator.Update(migration);
                Console.WriteLine("Migration {0} complete.", migration);
                Console.WriteLine();
            }
        }
    }
}
