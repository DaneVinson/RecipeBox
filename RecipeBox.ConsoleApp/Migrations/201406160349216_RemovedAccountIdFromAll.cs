namespace RecipeBox.ConsoleApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedAccountIdFromAll : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Accounts", "AccountId");
            DropColumn("dbo.Recipes", "AccountId2");
            DropColumn("dbo.Recipes", "AccountId");
            DropColumn("dbo.Tags", "AccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tags", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Recipes", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.Recipes", "AccountId2", c => c.String());
            AddColumn("dbo.Accounts", "AccountId", c => c.Int(nullable: false));
        }
    }
}
