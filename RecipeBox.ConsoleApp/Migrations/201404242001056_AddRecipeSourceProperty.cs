namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecipeSourceProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Source", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Source");
        }
    }
}
