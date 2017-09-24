namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientIdToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AccountId2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "AccountId2");
        }
    }
}
