namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountIdToRecipeAndTag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AccountId", c => c.String());
            AddColumn("dbo.Tags", "AccountId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "AccountId");
            DropColumn("dbo.Recipes", "AccountId");
        }
    }
}
