namespace RecipeBox.ConsoleApp.Migrations
{
    using Core;
    using DataContext;
    using Model.Enumerators;
    using Model.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    internal sealed class ConfigurationWithSeed : DbMigrationsConfiguration<RecipeBoxContext>
    {
        public ConfigurationWithSeed()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(RecipeBox.DataContext.RecipeBoxContext context)
        {
            string cookieMonsterId = Guid.NewGuid().ToString();

            var recipe = new Recipe()
            {
                AccountId = cookieMonsterId,
                Description = "The favorite of moms and kids for decades.",
                Directions = String.Format(
                                    "{0}\r\n\r\n{1}\r\n\r\n{2}\r\n\r\n{3}",
                                    "Arrange the bread face up on a clean surface.",
                                    "Spread the peanut butter on the first slice of bread.",
                                    "Spread the jelly on the second slice of bread.",
                                    "Lightly press the two slices together with the peanutbutter and jelly sides facing each other."),
                ImageFileName = "peanut-butter-jelly-sandwich.jpg",
                Ingredients = new List<Ingredient>(),
                Name = "Peanut Butter & Jelly",
                Notes = "Toast the sandwich lightly in a pan or try whole grain bread for interesting alternatives.",
                PreparationMinutes = 5,
                Servings = 1,
                Tags = new List<Tag>()
            };
            recipe.Ingredients.Add(new Ingredient() { Description = "White Bread", Quantity = "2", Units = "slice" });
            recipe.Ingredients.Add(new Ingredient() { Description = "Peanut Butter", Quantity = "2 1/2", Units = "tablespoon" });
            recipe.Ingredients.Add(new Ingredient() { Description = "Grape Jelly", Quantity = "2", Units = "tablespoon" });

            recipe.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "Sandwich" });
            recipe.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "Lunch" });
            recipe.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "Snack" });

            context.Recipes.Add(recipe);

            recipe = new Recipe()
            {
                AccountId = cookieMonsterId,
                Description = "Eggs and toast with a twist.",
                Directions = String.Format(
                                    "{0}\r\n\r\n{1}\r\n\r\n{2}\r\n\r\n{3}\r\n\r\n{4}",
                                    "Melt the butter in the bottom of a pan.",
                                    "Cut a whole in the center of the bread.",
                                    "Fry one side of the bread until it is toasted.",
                                    "Flip the break over and crack the egg into the hole in bread.",
                                    "Cook until the egg is the desired consistency."),
                ImageFileName = "cooked-egg-basket.jpg",
                Ingredients = new List<Ingredient>(),
                Name = "Egg in a Basket",
                Notes = null,
                PreparationMinutes = 10,
                Servings = 1,
                Source = "Mom",
                Tags = new List<Tag>()
            };
            recipe.Ingredients.Add(new Ingredient() { Description = "Wheat Bread", Quantity = "1", Units = "slice" });
            recipe.Ingredients.Add(new Ingredient() { Description = "Egg", Quantity = "1", Units = "large" });
            recipe.Ingredients.Add(new Ingredient() { Description = "Butter", Quantity = "1", Units = "tablespoon" });

            recipe.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "Breakfast" });

            context.Recipes.Add(recipe);

            context.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "Dinner" });

            string salt = CoreUtility.GenerateSalt();
            context.Accounts.Add(new Account()
            {
                Id = cookieMonsterId,
                AuthProvider = "RecipeBox",
                EmailAddress = "cookie.monster@sesamestreet.com",
                Password = CoreUtility.EncryptPassword("123456", salt),
                Salt = salt,
                Status = AccountStatus.Active.ToString(),
                UserName = "Cookie Monster",
                RowVersion = null
            });

            salt = CoreUtility.GenerateSalt();
            context.Accounts.Add(new Account()
            {
                Id = Guid.NewGuid().ToString(),
                AuthProvider = "RecipeBox",
                EmailAddress = "oscar.the.grouch@sesamestreet.com",
                Password = CoreUtility.EncryptPassword("123456", salt),
                Salt = salt,
                Status = AccountStatus.Suspended.ToString(),
                UserName = "Oscar the Grouch",
                RowVersion = null
            });

            ImportXmlRecipes(context, cookieMonsterId);

            context.SaveChanges();
        }

        private void ImportXmlRecipes(RecipeBoxContext context, string cookieMonsterId)
        {
            var anyRecipeTag = context.Tags.Add(new Tag() { AccountId = cookieMonsterId, Description = "My Recipes" });
            foreach (var recipe in context.Recipes)
            {
                recipe.Tags.Add(anyRecipeTag);
            }

            var xmlFiles = Directory.GetFiles("c:\\dane\\xmlrecipes", "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in xmlFiles)
            {
                XDocument document = null;
                using (StreamReader reader = new StreamReader(file))
                {
                    document = XDocument.Load(reader);
                }

                var header = document.Descendants().Where(d => d.Name.LocalName == "head").First();
                string title = header.Descendants().Where(d => d.Name.LocalName == "title").First().Value;
                string yield = header.Descendants().Where(d => d.Name.LocalName == "yield").First().Value;

                var steps = document.Descendants().Where(d => d.Name.LocalName == "step").ToList();
                var directions = new StringBuilder();
                steps.ForEach(s => directions.AppendLine(s.Value));

                var recipe = new Recipe()
                {
                    AccountId = cookieMonsterId,
                    Directions = directions.ToString(),
                    Ingredients = new List<Ingredient>(),
                    Name = title.Replace("\"", String.Empty).Trim(),
                    Servings = Convert.ToDouble(yield),
                    Tags = new List<Tag>()
                };
                if (recipe.Name.Equals("Green Mexican Salsa")) { recipe.ImageFileName = "green-mexican-salsa.jpg"; }
                recipe = context.Recipes.Add(recipe);
                recipe.Tags.Add(anyRecipeTag);

                var ingredients = document.Descendants().Where(d => d.Name.LocalName == "ing").ToArray();
                foreach (var ingredient in ingredients)
                {
                    string quantity = ingredient.Descendants().Where(d => d.Name.LocalName == "qty").First().Value;
                    if (String.IsNullOrWhiteSpace(quantity)) { quantity = "1"; }
                    string unit = ingredient.Descendants().Where(d => d.Name.LocalName == "unit").First().Value;
                    string item = ingredient.Descendants().Where(d => d.Name.LocalName == "item").First().Value;
                    recipe.Ingredients.Add(new Ingredient() { Description = item, Quantity = quantity, Units = unit });
                }
            }
        }
    }
}
